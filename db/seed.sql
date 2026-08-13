-- Mock data for the IT03 approval exercise.
-- Ten documents matching the IT 03-1 mockup, plus the approval log
-- entries for the rows that were already decided.

-- DocumentStatus (3 rows)
INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh") VALUES (1, 'PENDING', 'รออนุมัติ');
INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh") VALUES (2, 'APPROVED', 'อนุมัติ');
INSERT INTO "DocumentStatus" ("Id", "Code", "NameTh") VALUES (3, 'REJECTED', 'ไม่อนุมัติ');

-- Documents (10 rows)
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (1, 'รายการที่ 1', 'ขออนุมัติจัดซื้อวัสดุสำนักงาน ประจำเดือนสิงหาคม', 1, '2026-08-01 09:00:00', '2026-08-01 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (2, 'รายการที่ 2', 'ตรวจสอบเอกสารครบถ้วน อนุมัติตามระเบียบ', 2, '2026-08-02 09:00:00', '2026-08-03 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (3, 'รายการที่ 3', 'เอกสารแนบไม่ครบ ขาดใบเสนอราคา', 3, '2026-08-03 09:00:00', '2026-08-04 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (4, 'รายการที่ 4', 'ขออนุมัติเบิกค่าเดินทางไปราชการ', 1, '2026-08-04 09:00:00', '2026-08-04 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (5, 'รายการที่ 5', 'อยู่ในกรอบงบประมาณที่ได้รับจัดสรร', 2, '2026-08-05 09:00:00', '2026-08-06 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (6, 'รายการที่ 6', 'เกินวงเงินที่หน่วยงานกำหนด', 3, '2026-08-06 09:00:00', '2026-08-07 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (7, 'รายการที่ 7', 'ขออนุมัติจัดจ้างซ่อมบำรุงเครื่องปรับอากาศ', 1, '2026-08-07 09:00:00', '2026-08-07 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (8, 'รายการที่ 8', 'ขออนุมัติจัดอบรมบุคลากรภายใน', 1, '2026-08-08 09:00:00', '2026-08-08 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (9, 'รายการที่ 9', 'ขออนุมัติต่อสัญญาบริการระบบสารสนเทศ', 1, '2026-08-09 09:00:00', '2026-08-09 09:00:00');
INSERT INTO "Documents" ("Id", "DocumentName", "Reason", "StatusId", "CreatedAt", "UpdatedAt") VALUES (10, 'รายการที่ 10', 'ขออนุมัติจัดซื้อครุภัณฑ์คอมพิวเตอร์', 1, '2026-08-10 09:00:00', '2026-08-10 09:00:00');

-- ApprovalLog (4 rows)
INSERT INTO "ApprovalLog" ("Id", "DocumentId", "FromStatusId", "ToStatusId", "Reason", "ActionBy", "ActionAt") VALUES (1, 2, 1, 2, 'ตรวจสอบเอกสารครบถ้วน อนุมัติตามระเบียบ', 'somchai.k', '2026-08-03 09:00:00');
INSERT INTO "ApprovalLog" ("Id", "DocumentId", "FromStatusId", "ToStatusId", "Reason", "ActionBy", "ActionAt") VALUES (2, 3, 1, 3, 'เอกสารแนบไม่ครบ ขาดใบเสนอราคา', 'somchai.k', '2026-08-04 09:00:00');
INSERT INTO "ApprovalLog" ("Id", "DocumentId", "FromStatusId", "ToStatusId", "Reason", "ActionBy", "ActionAt") VALUES (3, 5, 1, 2, 'อยู่ในกรอบงบประมาณที่ได้รับจัดสรร', 'wanida.p', '2026-08-06 09:00:00');
INSERT INTO "ApprovalLog" ("Id", "DocumentId", "FromStatusId", "ToStatusId", "Reason", "ActionBy", "ActionAt") VALUES (4, 6, 1, 3, 'เกินวงเงินที่หน่วยงานกำหนด', 'wanida.p', '2026-08-07 09:00:00');
