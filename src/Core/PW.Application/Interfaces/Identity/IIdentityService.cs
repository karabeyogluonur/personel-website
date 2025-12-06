using PW.Application.Common.Models;

namespace PW.Application.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<OperationResult<int>> CreateUserAsync(string firstName, string lastName, string email, string password);
        Task<OperationResult<string>> CreateRoleAsync(string name, string description);
        Task<OperationResult<string>> AssignRoleAsync(int userId, string roleName);
        Task<int?> FindByEmailAsync(string email);
        Task<OperationResult> CheckPasswordSignInAsync(int userId, string password);
        Task<bool> IsInRoleAsync(int userId, string role);
        Task SignOutAsync();

    }
}
