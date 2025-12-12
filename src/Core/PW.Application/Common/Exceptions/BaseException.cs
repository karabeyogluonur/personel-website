namespace PW.Application.Common.Exceptions;

public abstract class BaseException : Exception
{
    public int StatusCode { get; }

    protected BaseException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
