using Microsoft.AspNetCore.Identity;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Identity.Entities;
using PW.Identity.Extensions;

namespace PW.Identity.Services
{
    public class IdentityService : IIdentityService
    {
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
        public async Task<OperationResult> CheckPasswordSignInAsync(int userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return OperationResult.Failure("User not found.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return OperationResult.Failure("User account is locked due to failed attempts.");

                if (result.IsNotAllowed)
                    return OperationResult.Failure("Sign in is not allowed for this user.");

                return OperationResult.Failure("Invalid password.");
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            return OperationResult.Success();
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

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
