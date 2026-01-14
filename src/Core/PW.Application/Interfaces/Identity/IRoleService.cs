using PW.Application.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity;

public interface IRoleService
{
   Task<OperationResult> CreateRoleAsync(CreateRoleDto createRoleDto);
   Task<List<string>> GetAllRolesAsync();
   Task<OperationResult> UpdateUserRolesAsync(UserRoleAssignmentDto userRoleAssignmentDto);
   Task<bool> IsInRoleAsync(int userId, string roleName);
}
