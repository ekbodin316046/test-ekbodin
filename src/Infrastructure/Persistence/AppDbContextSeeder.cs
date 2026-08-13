using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

// Mock data reproducing the ten rows of IT 03-1. Decided rows also get an
// ApprovalLog entry so the history view has content on a fresh database.
public static class AppDbContextSeeder
{
    private static readonly DateTime BaseDate = new(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Documents.AnyAsync(cancellationToken))
        {
            return;
        }

        var plan = new (int Row, DocumentStatusCode Status, string Reason, string? ActionBy)[]
        {
            (1, DocumentStatusCode.Pending, "ขออนุมัติจัดซื้อวัสดุสำนักงาน ประจำเดือนสิงหาคม", null),
            (2, DocumentStatusCode.Approved, "ตรวจสอบเอกสารครบถ้วน อนุมัติตามระเบียบ", "somchai.k"),
            (3, DocumentStatusCode.Rejected, "เอกสารแนบไม่ครบ ขาดใบเสนอราคา", "somchai.k"),
            (4, DocumentStatusCode.Pending, "ขออนุมัติเบิกค่าเดินทางไปราชการ", null),
            (5, DocumentStatusCode.Approved, "อยู่ในกรอบงบประมาณที่ได้รับจัดสรร", "wanida.p"),
            (6, DocumentStatusCode.Rejected, "เกินวงเงินที่หน่วยงานกำหนด", "wanida.p"),
            (7, DocumentStatusCode.Pending, "ขออนุมัติจัดจ้างซ่อมบำรุงเครื่องปรับอากาศ", null),
            (8, DocumentStatusCode.Pending, "ขออนุมัติจัดอบรมบุคลากรภายใน", null),
            (9, DocumentStatusCode.Pending, "ขออนุมัติต่อสัญญาบริการระบบสารสนเทศ", null),
            (10, DocumentStatusCode.Pending, "ขออนุมัติจัดซื้อครุภัณฑ์คอมพิวเตอร์", null),
        };

        foreach (var (row, status, reason, actionBy) in plan)
        {
            var createdAt = BaseDate.AddDays(row - 1);

            var document = new Document
            {
                DocumentName = $"รายการที่ {row}",
                Reason = reason,
                StatusId = (int)status,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
            };

            if (status != DocumentStatusCode.Pending)
            {
                var decidedAt = createdAt.AddDays(1);
                document.UpdatedAt = decidedAt;
                document.ApprovalLogs.Add(new ApprovalLog
                {
                    FromStatusId = (int)DocumentStatusCode.Pending,
                    ToStatusId = (int)status,
                    Reason = reason,
                    ActionBy = actionBy!,
                    ActionAt = decidedAt,
                });
            }

            context.Documents.Add(document);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
