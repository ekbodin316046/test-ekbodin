using Domain.Common;

namespace Domain.Entities;

// One row per status transition. Never updated or deleted, so the audit columns
// double as the record of who decided and when.
public class ApprovalLog : AuditableEntity
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document? Document { get; set; }

    public int FromStatusId { get; set; }
    public DocumentStatus? FromStatus { get; set; }

    public int ToStatusId { get; set; }
    public DocumentStatus? ToStatus { get; set; }

    public string Reason { get; set; } = string.Empty;
}
