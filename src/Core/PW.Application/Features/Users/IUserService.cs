using PW.Application.Utilities.Results;
using static PW.Application.Features.Users.Dtos.UserDto;

namespace PW.Application.Features.Users;

public interface IUserService
{
   Task<IList<UserSummaryDto>> GetAllUsersAsync();
   Task<UserDetailDto?> GetUserByIdAsync(int userId);
   Task<IList<string>> GetAllRolesAsync();
   Task<OperationResult> CreateUserAsync(UserCreateDto userCreateDto);
   Task<OperationResult> UpdateUserAsync(UserUpdateDto userUpdateDto);
   Task<OperationResult> DeleteUserAsync(int userId);
}
