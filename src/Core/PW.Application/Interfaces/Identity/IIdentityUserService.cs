using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Interfaces.Identity;

public interface IIdentityUserService
{
   Task<OperationResult<int>> CreateUserAsync(IdentityCreateUserDto createUserDto);
   Task<IdentityUserDto> GetUserByIdAsync(int userId);
   Task<IdentityUserDto> GetUserByEmailAsync(string email);
   Task<List<IdentityUserDto>> GetAllUsersAsync();
   Task<OperationResult> UpdateUserAsync(int userId, IdentityUserDto userDto);
   Task<OperationResult> DeleteUserAsync(int userId);
   Task<OperationResult> AdminResetUserPasswordAsync(IdentitySetPasswordDto setPasswordDto);
}
