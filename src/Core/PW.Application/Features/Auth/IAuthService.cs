using PW.Application.Features.Auth.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Auth;

public interface IAuthService
{
   Task<OperationResult> LoginAsync(LoginDto loginDto);
   Task LogoutAsync();
}
