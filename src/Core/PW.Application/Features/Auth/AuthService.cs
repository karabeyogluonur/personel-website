using PW.Application.Features.Auth.Dtos;
using PW.Application.Interfaces.Identity;
using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Auth;

public class AuthService : IAuthService
{
   private readonly IIdentityAuthService _identityAuthService;

   public AuthService(IIdentityAuthService identityAuthService)
   {
      _identityAuthService = identityAuthService;
   }

   public async Task<OperationResult> LoginAsync(LoginDto loginDto)
   {
      IdentityLoginDto identityLoginDto = new IdentityLoginDto
      {
         Email = loginDto.Email,
         Password = loginDto.Password,
         RememberMe = loginDto.RememberMe
      };

      return await _identityAuthService.LoginAsync(identityLoginDto);
   }

   public async Task LogoutAsync()
   {
      await _identityAuthService.LogoutAsync();
   }
}
