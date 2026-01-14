using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;
using PW.Identity.Entities;

namespace PW.Identity.Services;

public class IdentityAuthService : IIdentityAuthService
{
   private readonly SignInManager<ApplicationUser> _signInManager;
   private readonly UserManager<ApplicationUser> _userManager;
   private readonly ILogger<IdentityAuthService> _logger;

   public IdentityAuthService(
       SignInManager<ApplicationUser> signInManager,
       UserManager<ApplicationUser> userManager,
       ILogger<IdentityAuthService> logger)
   {
      _signInManager = signInManager;
      _userManager = userManager;
      _logger = logger;
   }

   public async Task<OperationResult> LoginAsync(IdentityLoginDto loginDto)
   {
      if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

      ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginDto.Email);
      if (applicationUser == null)
         return OperationResult.Failure("Invalid email or password.", OperationErrorType.Unauthorized);

      SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(applicationUser, loginDto.Password, lockoutOnFailure: true);

      if (signInResult.Succeeded)
      {
         await _signInManager.SignInAsync(applicationUser, isPersistent: loginDto.RememberMe);
         _logger.LogInformation("User {Email} logged in successfully.", applicationUser.Email);
         return OperationResult.Success();
      }

      if (signInResult.IsLockedOut)
         return OperationResult.Failure("Account is locked.", OperationErrorType.BusinessRule);

      return OperationResult.Failure("Invalid email or password.", OperationErrorType.Unauthorized);
   }

   public async Task LogoutAsync()
   {
      await _signInManager.SignOutAsync();
      _logger.LogInformation("User logged out.");
   }
}
