using PW.Application.Common.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<OperationResult<int>> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<OperationResult> UpdateUserAsync(int userId, UserDto userDto);
        Task<OperationResult> DeleteUserAsync(int userId);
        Task<OperationResult> AdminResetUserPasswordAsync(SetPasswordDto setPasswordDto);
    }
}
