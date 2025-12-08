using PW.Application.Common.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<OperationResult<int>> CreateUserAsync(string firstName, string lastName, string email, string password);
        Task<int?> FindByEmailAsync(string email);
        Task<OperationResult> DeleteUserAsync(int userId);
        Task<OperationResult> UpdateUserAsync(UserDto userDto);
        Task<OperationResult> ChangeUserPasswordAsync(int userId, string newPassword);
    }
}
