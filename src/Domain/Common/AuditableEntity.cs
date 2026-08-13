namespace Domain.Common;

// Every table carries the same six audit columns, following the enterprise
// database convention. Program holds the screen code that touched the row, which is what
// makes an audit trail readable when several screens write the same table.
public abstract class AuditableEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedProgram { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string UpdatedProgram { get; set; } = string.Empty;

    public void StampCreated(string user, string program, DateTime now)
    {
        CreatedBy = user;
        CreatedDate = now;
        CreatedProgram = program;
        StampUpdated(user, program, now);
    }

    public void StampUpdated(string user, string program, DateTime now)
    {
        UpdatedBy = user;
        UpdatedDate = now;
        UpdatedProgram = program;
    }
}
