using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Document
{
    public int Id { get; set; }
    public string DocumentName { get; set; } = string.Empty;

    // Current reason shown in the list. Seeded with the submitted reason, then
    // overwritten by each decision; ApprovalLogs keeps the full history.
    public string? Reason { get; set; }

    public int StatusId { get; set; }
    public DocumentStatus? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ApprovalLog> ApprovalLogs { get; set; } = new List<ApprovalLog>();

    public bool IsPending => StatusId == (int)DocumentStatusCode.Pending;

    public ApprovalLog ChangeStatus(
        DocumentStatusCode toStatus,
        string reason,
        string actionBy,
        DateTime now)
    {
        if (toStatus is not (DocumentStatusCode.Approved or DocumentStatusCode.Rejected))
        {
            throw new BusinessRuleException(
                $"ไม่สามารถเปลี่ยนสถานะเป็น '{toStatus.ToNameTh()}' ได้");
        }

        // Lives here, not in the controller or the UI, so it holds for every caller.
        if (!IsPending)
        {
            throw new BusinessRuleException(
                $"เอกสาร '{DocumentName}' มีสถานะ '{((DocumentStatusCode)StatusId).ToNameTh()}' อยู่แล้ว " +
                "ไม่สามารถดำเนินการซ้ำได้");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleException("กรุณากรอกเหตุผล");
        }

        var trimmedReason = reason.Trim();

        var log = new ApprovalLog
        {
            DocumentId = Id,
            FromStatusId = StatusId,
            ToStatusId = (int)toStatus,
            Reason = trimmedReason,
            ActionBy = actionBy,
            ActionAt = now,
        };

        StatusId = (int)toStatus;
        Reason = trimmedReason;
        UpdatedAt = now;
        ApprovalLogs.Add(log);

        return log;
    }
}
