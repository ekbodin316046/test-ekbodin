namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException Document(int id) => new($"ไม่พบเอกสารรหัส {id}");
}
