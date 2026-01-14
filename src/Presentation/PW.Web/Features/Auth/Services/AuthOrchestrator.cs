using AutoMapper;
using PW.Application.Features.Auth;
using PW.Application.Features.Auth.Dtos;
using PW.Application.Utilities.Results;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services;

public class AuthOrchestrator : IAuthOrchestrator
{
   private readonly IAuthService _authService;
   private readonly IMapper _mapper;

   public AuthOrchestrator(IAuthService authService, IMapper mapper)
   {
      _authService = authService;
      _mapper = mapper;
   }

   public async Task<OperationResult> LoginAsync(LoginViewModel loginViewModel)
   {
      if (loginViewModel == null)
         throw new ArgumentNullException(nameof(loginViewModel));

      LoginDto loginDto = _mapper.Map<LoginDto>(loginViewModel);

      return await _authService.LoginAsync(loginDto);
   }

   public async Task LogoutAsync()
   {
      await _authService.LogoutAsync();
   }
}
