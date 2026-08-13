namespace Domain.Exceptions;

// A domain invariant was violated, as opposed to bad input. Maps to HTTP 409.
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
