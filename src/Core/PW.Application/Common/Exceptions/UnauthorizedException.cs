namespace PW.Application.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message, 401)
    {
    }
}
