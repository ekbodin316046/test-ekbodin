namespace Domain.Entities;

// One row per status transition. Never updated or deleted.
public class ApprovalLog
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document? Document { get; set; }

    public int FromStatusId { get; set; }
    public DocumentStatus? FromStatus { get; set; }

    public int ToStatusId { get; set; }
    public DocumentStatus? ToStatus { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string ActionBy { get; set; } = string.Empty;
    public DateTime ActionAt { get; set; }
}
