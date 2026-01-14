using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Interfaces.Identity;

public interface IIdentityRoleService
{
   Task<OperationResult> CreateRoleAsync(IdentityCreateRoleDto createRoleDto);
   Task<List<string>> GetAllRolesAsync();
   Task<OperationResult> UpdateUserRolesAsync(IdentityUserRoleAssignmentDto userRoleAssignmentDto);
   Task<bool> IsInRoleAsync(int userId, string roleName);
}
