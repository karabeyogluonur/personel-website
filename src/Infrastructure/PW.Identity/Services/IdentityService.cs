using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;
using PW.Identity.Extensions;

namespace PW.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        //TODO: Methods will be separated according to their fields.

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<OperationResult<int>> CreateUserAsync(string firstName, string lastName, string email, string password)
        {
            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);
            return result.ToOperationResult(user.Id);
        }
        public async Task<int?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }
        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        public async Task<OperationResult<string>> CreateRoleAsync(string name, string description)
        {
            var role = new ApplicationRole
            {
                Name = name,
                Description = description
            };

            var result = await _roleManager.CreateAsync(role);
            return result.ToOperationResult(role.Id.ToString());
        }
        public async Task<OperationResult<string>> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return OperationResult<string>.Failure("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return OperationResult<string>.Failure("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.ToOperationResult("Role assigned successfully.");
        }
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleNames = roles?.ToList() ?? new List<string>()
                });
            }

            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleNames = roles?.ToList() ?? new List<string>()
            };
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                .Select(role => role.Name)
                .ToListAsync();
        }
        public async Task<OperationResult> UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user is null)
                return OperationResult.Failure("User not found.");

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result.ToOperationResult();

            return OperationResult.Success();
        }
        public async Task<OperationResult> UpdateUserRolesAsync(int userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return OperationResult.Failure("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return removeResult.ToOperationResult();

            if (roles == null || !roles.Any())
                return OperationResult.Success();

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
                return addResult.ToOperationResult();

            return OperationResult.Success();
        }
        public async Task<OperationResult> ChangeUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return OperationResult.Failure("User not found.");

            var removeResult = await _userManager.RemovePasswordAsync(user);

            if (!removeResult.Succeeded)
                return removeResult.ToOperationResult();

            var addResult = await _userManager.AddPasswordAsync(user, newPassword);

            if (!addResult.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return addResult.ToOperationResult();
            }


            return OperationResult.Success();
        }
        public async Task<OperationResult> AssignRoleAsync(int userId, List<string> roleNames)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return OperationResult.Failure("User not found.");

            if (roleNames == null || !roleNames.Any())
                return OperationResult.Success();

            // Rollerin varlığını kontrol et
            var notExists = new List<string>();
            foreach (var role in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    notExists.Add(role);
            }

            if (notExists.Any())
                return OperationResult.Failure($"Roles do not exist: {string.Join(", ", notExists)}");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roleNames.Except(currentRoles).ToList();

            if (!rolesToAdd.Any())
                return OperationResult.Success();

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
                return result.ToOperationResult();

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return OperationResult.Failure("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return OperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());

            return OperationResult.Success();
        }

    }
}
