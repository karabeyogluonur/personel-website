using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;
using PW.Identity.Extensions;

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
            if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
            {
                return OperationResult.Failure($"Role '{createRoleDto.Name}' already exists.");
            }

            var role = new ApplicationRole
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description
            };

            var result = await _roleManager.CreateAsync(role);

            return result.ToOperationResult();
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
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return false;

            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<OperationResult> UpdateUserRolesAsync(UserRoleAssignmentDto assignmentDto)
        {
            var user = await _userManager.FindByIdAsync(assignmentDto.UserId.ToString());
            if (user is null)
                return OperationResult.Failure("User not found.");

            var requestedRoles = assignmentDto.RoleNames ?? new List<string>();
            var currentRoles = await _userManager.GetRolesAsync(user);

            var nonExistentRoles = requestedRoles
                .Where(r => !_roleManager.Roles.Any(existingRole => existingRole.Name == r))
                .ToList();

            if (nonExistentRoles.Any())
                return OperationResult.Failure($"The following roles do not exist: {string.Join(", ", nonExistentRoles)}");

            var rolesToRemove = currentRoles.Except(requestedRoles).ToList();
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return removeResult.ToOperationResult();
            }

            var rolesToAdd = requestedRoles.Except(currentRoles).ToList();
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    return addResult.ToOperationResult();
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return OperationResult.Success();
        }
    }
}
