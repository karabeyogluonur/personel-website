using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;

namespace PW.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return OperationResult.Failure("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: loginDto.RememberMe);
                _logger.LogInformation($"User {user.Email} logged in successfully.");
                return OperationResult.Success();
            }

            if (result.IsLockedOut)
                return OperationResult.Failure("Account is locked. Please try again later.");

            return OperationResult.Failure("Invalid email or password.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
    }
}
