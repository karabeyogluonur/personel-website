using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Interfaces.Identity;

public interface IIdentityAuthService
{
   Task<OperationResult> LoginAsync(IdentityLoginDto loginDto);
   Task LogoutAsync();
}
