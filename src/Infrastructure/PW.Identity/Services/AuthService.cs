using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Models;
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
            if (loginDto is null)
                throw new ArgumentNullException(nameof(loginDto));

            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return OperationResult.Failure("Email and Password are required.", OperationErrorType.ValidationError);

            var applicationUser = await _userManager.FindByEmailAsync(loginDto.Email);

            if (applicationUser is null)
                return OperationResult.Failure("Invalid email or password.", OperationErrorType.Unauthorized);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(applicationUser, loginDto.Password, lockoutOnFailure: true);

            if (signInResult.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, isPersistent: loginDto.RememberMe);
                _logger.LogInformation("User {Email} logged in successfully.", applicationUser.Email);
                return OperationResult.Success();
            }

            if (signInResult.IsLockedOut)
                return OperationResult.Failure("Account is locked due to multiple failed attempts. Please try again later.", OperationErrorType.BusinessRule);

            return OperationResult.Failure("Invalid email or password.", OperationErrorType.Unauthorized);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
    }
}
