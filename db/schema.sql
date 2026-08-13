CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "DocumentStatus" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentStatus" PRIMARY KEY,
    "Code" TEXT NOT NULL,
    "NameTh" TEXT NOT NULL
);

CREATE TABLE "Documents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Documents" PRIMARY KEY AUTOINCREMENT,
    "DocumentName" TEXT NOT NULL,
    "Reason" TEXT NULL,
    "StatusId" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Documents_DocumentStatus_StatusId" FOREIGN KEY ("StatusId") REFERENCES "DocumentStatus" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ApprovalLog" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApprovalLog" PRIMARY KEY AUTOINCREMENT,
    "DocumentId" INTEGER NOT NULL,
    "FromStatusId" INTEGER NOT NULL,
    "ToStatusId" INTEGER NOT NULL,
    "Reason" TEXT NOT NULL,
    "ActionBy" TEXT NOT NULL,
    "ActionAt" TEXT NOT NULL,
    CONSTRAINT "FK_ApprovalLog_DocumentStatus_FromStatusId" FOREIGN KEY ("FromStatusId") REFERENCES "DocumentStatus" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ApprovalLog_DocumentStatus_ToStatusId" FOREIGN KEY ("ToStatusId") REFERENCES "DocumentStatus" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ApprovalLog_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE CASCADE
);

INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh")
VALUES (1, 'PENDING', 'รออนุมัติ');
SELECT changes();

INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh")
VALUES (2, 'APPROVED', 'อนุมัติ');
SELECT changes();

INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh")
VALUES (3, 'REJECTED', 'ไม่อนุมัติ');
SELECT changes();


CREATE INDEX "IX_ApprovalLog_DocumentId" ON "ApprovalLog" ("DocumentId");

CREATE INDEX "IX_ApprovalLog_FromStatusId" ON "ApprovalLog" ("FromStatusId");

CREATE INDEX "IX_ApprovalLog_ToStatusId" ON "ApprovalLog" ("ToStatusId");

CREATE INDEX "IX_Documents_StatusId" ON "Documents" ("StatusId");

CREATE UNIQUE INDEX "IX_DocumentStatus_Code" ON "DocumentStatus" ("Code");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260813114449_InitialCreate', '10.0.11');

COMMIT;

