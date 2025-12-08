using PW.Application.Common.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<List<string>> GetAllRolesAsync();
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<OperationResult<int>> CreateUserAsync(string firstName, string lastName, string email, string password);
        Task<OperationResult<string>> CreateRoleAsync(string name, string description);
        Task<OperationResult<string>> AssignRoleAsync(int userId, string roleName);
        Task<OperationResult> AssignRoleAsync(int userId, List<string> roleNames);
        Task<int?> FindByEmailAsync(string email);
        Task<bool> IsInRoleAsync(int userId, string role);
        Task<OperationResult> DeleteUserAsync(int userId);
        Task<OperationResult> UpdateUserAsync(UserDto userDto);
        Task<OperationResult> UpdateUserRolesAsync(int userId, List<string> roles);
        Task<OperationResult> ChangeUserPasswordAsync(int userId, string newPassword);
    }
}
