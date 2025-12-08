using PW.Application.Common.Models;
using PW.Application.Models.Dtos.Identity;

namespace PW.Application.Interfaces.Identity
{
    public interface IRoleService
    {
        Task<OperationResult> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<List<string>> GetAllRolesAsync();
        Task<OperationResult> UpdateUserRolesAsync(UserRoleAssignmentDto assignmentDto);
        Task<bool> IsInRoleAsync(int userId, string roleName);
    }
}
