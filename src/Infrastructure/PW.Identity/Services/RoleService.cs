using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Models;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;

namespace PW.Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<OperationResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (createRoleDto is null)
                throw new ArgumentNullException(nameof(createRoleDto));

            if (string.IsNullOrWhiteSpace(createRoleDto.Name))
                return OperationResult.Failure("Role name cannot be empty.", OperationErrorType.ValidationError);

            if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
                return OperationResult.Failure($"Role '{createRoleDto.Name}' already exists.", OperationErrorType.Conflict);

            var applicationRole = new ApplicationRole
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description
            };

            var identityResult = await _roleManager.CreateAsync(applicationRole);

            if (!identityResult.Succeeded)
                return OperationResult.Failure(identityResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "Error creating role.", OperationErrorType.Technical);

            return OperationResult.Success();
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .Select(role => role.Name)
                .ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            if (userId <= 0) return false;
            if (string.IsNullOrWhiteSpace(roleName)) return false;

            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return false;

            return await _userManager.IsInRoleAsync(applicationUser, roleName);
        }

        public async Task<OperationResult> UpdateUserRolesAsync(UserRoleAssignmentDto userRoleAssignmentDto)
        {
            if (userRoleAssignmentDto is null)
                throw new ArgumentNullException(nameof(userRoleAssignmentDto));

            if (userRoleAssignmentDto.UserId <= 0)
                return OperationResult.Failure("Invalid user ID.", OperationErrorType.ValidationError);

            var applicationUser = await _userManager.FindByIdAsync(userRoleAssignmentDto.UserId.ToString());

            if (applicationUser is null)
                return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

            var requestedRoles = userRoleAssignmentDto.RoleNames ?? new List<string>();
            var currentRoles = await _userManager.GetRolesAsync(applicationUser);

            var allSystemRoles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
            var invalidRoles = requestedRoles.Except(allSystemRoles).ToList();

            if (invalidRoles.Any())
                return OperationResult.Failure($"The following roles do not exist: {string.Join(", ", invalidRoles)}", OperationErrorType.NotFound);

            var rolesToRemove = currentRoles.Except(requestedRoles).ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(applicationUser, rolesToRemove);
                if (!removeResult.Succeeded)
                    return OperationResult.Failure("Failed to remove old roles.", OperationErrorType.Technical);
            }

            var rolesToAdd = requestedRoles.Except(currentRoles).ToList();
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(applicationUser, rolesToAdd);

                if (!addResult.Succeeded)
                    return OperationResult.Failure("Failed to add new roles.", OperationErrorType.Technical);
            }

            await _userManager.UpdateSecurityStampAsync(applicationUser);

            return OperationResult.Success();
        }
    }
}
