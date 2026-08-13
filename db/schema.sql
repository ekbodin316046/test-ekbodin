CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "document_status" (
    "status_id" INTEGER NOT NULL CONSTRAINT "PK_document_status" PRIMARY KEY,
    "status_code" TEXT NOT NULL,
    "status_name_tha" TEXT NOT NULL,
    "created_by" TEXT NOT NULL,
    "created_date" TEXT NOT NULL,
    "created_program" TEXT NOT NULL,
    "updated_by" TEXT NOT NULL,
    "updated_date" TEXT NOT NULL,
    "updated_program" TEXT NOT NULL
);

CREATE TABLE "documents" (
    "document_id" INTEGER NOT NULL CONSTRAINT "PK_documents" PRIMARY KEY AUTOINCREMENT,
    "document_name" TEXT NOT NULL,
    "reason" TEXT NULL,
    "status_id" INTEGER NOT NULL,
    "created_by" TEXT NOT NULL,
    "created_date" TEXT NOT NULL,
    "created_program" TEXT NOT NULL,
    "updated_by" TEXT NOT NULL,
    "updated_date" TEXT NOT NULL,
    "updated_program" TEXT NOT NULL,
    CONSTRAINT "FK_documents_document_status_status_id" FOREIGN KEY ("status_id") REFERENCES "document_status" ("status_id") ON DELETE RESTRICT
);

CREATE TABLE "approval_log" (
    "approval_log_id" INTEGER NOT NULL CONSTRAINT "PK_approval_log" PRIMARY KEY AUTOINCREMENT,
    "document_id" INTEGER NOT NULL,
    "from_status_id" INTEGER NOT NULL,
    "to_status_id" INTEGER NOT NULL,
    "reason" TEXT NOT NULL,
    "created_by" TEXT NOT NULL,
    "created_date" TEXT NOT NULL,
    "created_program" TEXT NOT NULL,
    "updated_by" TEXT NOT NULL,
    "updated_date" TEXT NOT NULL,
    "updated_program" TEXT NOT NULL,
    CONSTRAINT "FK_approval_log_document_status_from_status_id" FOREIGN KEY ("from_status_id") REFERENCES "document_status" ("status_id") ON DELETE RESTRICT,
    CONSTRAINT "FK_approval_log_document_status_to_status_id" FOREIGN KEY ("to_status_id") REFERENCES "document_status" ("status_id") ON DELETE RESTRICT,
    CONSTRAINT "FK_approval_log_documents_document_id" FOREIGN KEY ("document_id") REFERENCES "documents" ("document_id") ON DELETE CASCADE
);

INSERT INTO "document_status" ("status_id", "status_code", "created_by", "created_date", "created_program", "status_name_tha", "updated_by", "updated_date", "updated_program")
VALUES (1, 'PENDING', 'SYSTEM', '2026-08-01 09:00:00', 'SEED', 'รออนุมัติ', 'SYSTEM', '2026-08-01 09:00:00', 'SEED');
SELECT changes();

INSERT INTO "document_status" ("status_id", "status_code", "created_by", "created_date", "created_program", "status_name_tha", "updated_by", "updated_date", "updated_program")
VALUES (2, 'APPROVED', 'SYSTEM', '2026-08-01 09:00:00', 'SEED', 'อนุมัติ', 'SYSTEM', '2026-08-01 09:00:00', 'SEED');
SELECT changes();

INSERT INTO "document_status" ("status_id", "status_code", "created_by", "created_date", "created_program", "status_name_tha", "updated_by", "updated_date", "updated_program")
VALUES (3, 'REJECTED', 'SYSTEM', '2026-08-01 09:00:00', 'SEED', 'ไม่อนุมัติ', 'SYSTEM', '2026-08-01 09:00:00', 'SEED');
SELECT changes();


CREATE INDEX "IX_approval_log_document_id" ON "approval_log" ("document_id");

CREATE INDEX "IX_approval_log_from_status_id" ON "approval_log" ("from_status_id");

CREATE INDEX "IX_approval_log_to_status_id" ON "approval_log" ("to_status_id");

CREATE UNIQUE INDEX "IX_document_status_status_code" ON "document_status" ("status_code");

CREATE INDEX "IX_documents_status_id" ON "documents" ("status_id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260813144958_InitialCreate', '10.0.11');

COMMIT;

