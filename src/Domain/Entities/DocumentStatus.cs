using Domain.Common;

namespace Domain.Entities;

public class DocumentStatus : AuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameTh { get; set; } = string.Empty;
}
