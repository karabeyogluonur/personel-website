namespace Application.Common.Exceptions;

public class ValidationException : BaseException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Validation error occurred.", 400)
    {
        Errors = errors;
    }
}
