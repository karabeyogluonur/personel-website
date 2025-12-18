using PW.Application.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<OperationResult> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
    }
}
