using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;
using PW.Identity.Entities;

namespace PW.Identity.Services;

public class IdentityRoleService : IIdentityRoleService
{
   private readonly RoleManager<ApplicationRole> _roleManager;
   private readonly UserManager<ApplicationUser> _userManager;

   public IdentityRoleService(
       RoleManager<ApplicationRole> roleManager,
       UserManager<ApplicationUser> userManager)
   {
      _roleManager = roleManager;
      _userManager = userManager;
   }

   public async Task<OperationResult> CreateRoleAsync(IdentityCreateRoleDto createRoleDto)
   {
      if (createRoleDto == null) throw new ArgumentNullException(nameof(createRoleDto));

      if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
         return OperationResult.Failure($"Role '{createRoleDto.Name}' already exists.", OperationErrorType.Conflict);

      ApplicationRole applicationRole = new ApplicationRole
      {
         Name = createRoleDto.Name,
         Description = createRoleDto.Description
      };

      IdentityResult identityResult = await _roleManager.CreateAsync(applicationRole);
      return identityResult.Succeeded
          ? OperationResult.Success()
          : OperationResult.Failure("Error creating role.", OperationErrorType.Technical);
   }

   public async Task<List<string>> GetAllRolesAsync()
   {
      return await _roleManager.Roles
          .AsNoTracking()
          .Select(role => role.Name!)
          .ToListAsync();
   }

   public async Task<bool> IsInRoleAsync(int userId, string roleName)
   {
      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(userId.ToString());
      return applicationUser != null && await _userManager.IsInRoleAsync(applicationUser, roleName);
   }

   public async Task<OperationResult> UpdateUserRolesAsync(IdentityUserRoleAssignmentDto userRoleAssignmentDto)
   {
      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(userRoleAssignmentDto.UserId.ToString());
      if (applicationUser == null) return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

      IList<string> currentRoles = await _userManager.GetRolesAsync(applicationUser);
      List<string> requestedRoles = userRoleAssignmentDto.RoleNames ?? new List<string>();

      IEnumerable<string> rolesToRemove = currentRoles.Except(requestedRoles);
      if (rolesToRemove.Any())
         await _userManager.RemoveFromRolesAsync(applicationUser, rolesToRemove);

      IEnumerable<string> rolesToAdd = requestedRoles.Except(currentRoles);
      if (rolesToAdd.Any())
         await _userManager.AddToRolesAsync(applicationUser, rolesToAdd);

      await _userManager.UpdateSecurityStampAsync(applicationUser);
      return OperationResult.Success();
   }
}
