# IT03 — Document Approval — Design

Date: 2026-08-13

## 1. Requirement (from the exam brief)

Build a UI with three screens driven by one document list:

- **IT 03-1** — a table of documents. Columns: selection checkbox, `รายการ`, `เหตุผล`, `สถานะเอกสาร`.
  Three statuses exist: `รออนุมัติ`, `อนุมัติ`, `ไม่อนุมัติ`. A document that has already been
  approved or rejected cannot be selected again — its checkbox renders grey and disabled.
- **IT 03-2** — select one or more pending rows, press `อนุมัติ`. A modal titled
  `ยืนยันการอนุมัติ` opens with a `เหตุผล :` textarea. Pressing `อนุมัติ` writes the reason and
  moves the documents to `อนุมัติ`. Pressing `ยกเลิก` closes the modal without saving.
- **IT 03-3** — same flow for `ไม่อนุมัติ`, modal titled `ยืนยันการไม่อนุมัติ`, target status
  `ไม่อนุมัติ`.

Database structure is left to the implementer. Data is mock data.

### Reading the mockups

Two details drive the design and are not stated in the prose:

1. The `อนุมัติ` / `ไม่อนุมัติ` buttons sit **above** the table, not on each row, and every row has a
   checkbox with a select-all checkbox in the header. The operation is therefore a **bulk action**:
   tick N rows, press one button, enter one reason, commit all N.
2. The `เหตุผล` column shows a value on **every** row, including rows still in `รออนุมัติ`. A pending
   row has no approval history yet, so this column cannot be sourced purely from the approval log.

## 2. Scope

In scope: the document list, bulk approve, bulk reject, the duplicate-action guard, a full approval
audit trail, and a history view per document.

Out of scope: authentication. The brief does not ask for it. An `ICurrentUserAccessor` seam records
who acted, resolved from an `X-User` header and defaulting to `demo.user`, so adding real auth later
touches one class.

## 3. Architecture

Clean Architecture, four layers, dependencies pointing inward only:

```
Api  ──▶  Application  ──▶  Domain
             ▲
Infrastructure ┘        (implements Application interfaces)
```

Requests never reach a handler directly. Controllers publish a message through **MediatR** and a
behaviour pipeline wraps every command:

```
Controller → IMediator → ValidationBehaviour → TransactionBehaviour → Handler → Domain entity
```

- `ValidationBehaviour` runs FluentValidation rules and fails before any database work.
- `TransactionBehaviour` opens a transaction for requests marked `ICommand`, commits on success,
  rolls back on exception. Queries bypass it.

One file per use case under `Application/Features/IT/IT03/`: `GetDocumentList.cs`, `Approve.cs`,
`Reject.cs`, `GetApprovalHistory.cs`.

### Where the business rule lives

The rule "an approved or rejected document cannot be acted on again" belongs to the entity, not to a
controller and not to the UI:

```csharp
// Domain/Entities/Document.cs
public ApprovalLog ChangeStatus(DocumentStatusCode to, string reason, string actionBy, DateTime now)
{
    if (StatusId != (int)DocumentStatusCode.Pending)
        throw new BusinessRuleException(
            $"เอกสาร '{DocumentName}' มีสถานะ {StatusNameTh} อยู่แล้ว ไม่สามารถดำเนินการซ้ำได้");

    var log = new ApprovalLog { /* From = StatusId, To = to, reason, actionBy, now */ };
    StatusId  = (int)to;
    Reason    = reason;
    UpdatedAt = now;
    return log;
}
```

`Approve` and `Reject` handlers become thin loops over the selected ids. The guard is enforced
server-side regardless of what the UI allows, and it is unit-testable with no database.

## 4. Data model

SQLite. Three tables.

**Documents**

| Column | Type | Notes |
|---|---|---|
| Id | INTEGER PK | identity |
| DocumentName | nvarchar(200) | `รายการที่ 1` … `รายการที่ 10` |
| Reason | nvarchar(500) | current reason; see below |
| StatusId | INTEGER FK → DocumentStatus | |
| CreatedAt / UpdatedAt | datetime | |

**DocumentStatus** (master)

| Id | Code | NameTh |
|---|---|---|
| 1 | PENDING | รออนุมัติ |
| 2 | APPROVED | อนุมัติ |
| 3 | REJECTED | ไม่อนุมัติ |

**ApprovalLog**

| Column | Type |
|---|---|
| Id | INTEGER PK |
| DocumentId | FK → Documents |
| FromStatusId / ToStatusId | FK → DocumentStatus |
| Reason | nvarchar(500) |
| ActionBy | nvarchar(100) |
| ActionAt | datetime |

### Two decisions worth recording

**`Amount` is not in the model.** The original schema sketch carried
`Amount (decimal) -- คอลัมน์ "เหตุผล/xxxxx"`. The mockup has no monetary column; the third column is
`เหตุผล` displaying the placeholder `xxxxx`. That is free text, so the column is `Reason nvarchar`
and no decimal field exists.

**`Documents.Reason` is a denormalised current value.** Because the mockup shows a reason on pending
rows, the seed populates `Reason` for all ten documents. An approve or reject inserts an
`ApprovalLog` row **and** overwrites `Documents.Reason` with the new reason. The list query stays a
single-table read, and `ApprovalLog` remains the complete history rather than a decorative table.

### Seed data

Ten documents matching IT 03-1: rows 1, 4, 7, 8, 9, 10 pending; rows 2 and 5 approved; rows 3 and 6
rejected. Approved and rejected rows get a matching `ApprovalLog` entry so the history view has
content on first run.

The database ships three ways so a reviewer can inspect tables without running anything:
`db/schema.sql` and `db/seed.sql` are readable directly on GitHub, and `db/app.db` opens in DB Browser
for SQLite or the VS Code SQLite extension.

## 5. API

| Method | Route | Body |
|---|---|---|
| GET | `/api/it03/documents` | — |
| POST | `/api/it03/documents/approve` | `{ documentIds: [1,4], reason: "..." }` |
| POST | `/api/it03/documents/reject` | `{ documentIds: [7], reason: "..." }` |
| GET | `/api/it03/documents/{id}/logs` | — |

Approve and reject take an array to match the bulk UI and run inside one transaction. If any
selected document violates the guard, the whole batch rolls back and the response names the
offending document. Errors surface as RFC 7807 `ProblemDetails` via exception middleware:
`BusinessRuleException` → 409, `ValidationException` → 400, `NotFoundException` → 404.

Swagger UI is enabled so the endpoints are explorable without the frontend.

## 6. Frontend

Angular 22, standalone components, signals, `@if` / `@for`. Hand-written CSS rather than a component
library — the mockup is visually specific and plain CSS matches it exactly with less setup.

```
web/src/app/
  core/                 http interceptor (error surfacing, loading state)
  shared/               modal, status badge
  features/it03/
    it03-list.component.ts        IT 03-1
    approval-dialog.component.ts  IT 03-2 and IT 03-3 — one component, title and
                                  confirm button vary by mode
    it03.service.ts
```

Behaviour taken from the mockups: non-pending checkboxes are grey and disabled; the header checkbox
selects only pending rows; both top buttons are disabled while nothing is selected; the modal confirm
button is disabled until a reason is entered; `ยกเลิก` closes without saving. Colours follow the
mockup — green header bar, green approve, red reject, blue table header, blue modal title bar.

## 7. Tests

xUnit against `Document.ChangeStatus`, no database required:

- approving a pending document sets status 2 and returns a log with `From=1, To=2`
- rejecting a pending document sets status 3 and returns a log with `From=1, To=3`
- approving an already-approved document throws `BusinessRuleException`
- rejecting an already-rejected document throws `BusinessRuleException`
- the returned log carries the supplied reason and `ActionBy`

## 8. Repository layout

```
It03Approval.sln
src/Domain/  src/Application/  src/Infrastructure/  src/Api/
tests/Domain.Tests/
web/
db/schema.sql  db/seed.sql  db/app.db
docs/specs/
README.md
```

## 9. Delivery order

Deadline is 23:59 on 2026-08-13. Committed in stages so the repository always holds working code:
solution scaffold → domain → persistence and seed → application handlers → API → tests → Angular
list → approval dialog → history view → README.

If time runs short, the mandatory core is IT 03-1/2/3 plus the API and database. Tests, the history
view, and README polish are the tail that can be trimmed.
