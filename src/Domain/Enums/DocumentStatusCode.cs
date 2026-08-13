namespace Domain.Enums;

// Values are the DocumentStatus primary keys.
public enum DocumentStatusCode
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
}

public static class DocumentStatusCodeExtensions
{
    public static string ToNameTh(this DocumentStatusCode code) => code switch
    {
        DocumentStatusCode.Pending => "รออนุมัติ",
        DocumentStatusCode.Approved => "อนุมัติ",
        DocumentStatusCode.Rejected => "ไม่อนุมัติ",
        _ => code.ToString(),
    };
}
