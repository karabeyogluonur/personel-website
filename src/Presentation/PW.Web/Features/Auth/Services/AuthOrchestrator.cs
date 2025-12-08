// PW.Web.Features.Auth.Services/AuthOrchestrator.cs
using AutoMapper;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services
{
    public class AuthOrchestrator : IAuthOrchestrator
    {
        private readonly IIdentityService _identityService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthOrchestrator(IIdentityService identityService, IAuthService authService, IMapper mapper)
        {
            _identityService = identityService;
            _authService = authService;
            _mapper = mapper;

        }

        public async Task<OperationResult> LoginAsync(LoginViewModel loginViewModel)
        {
            int? userId = await _identityService.FindByEmailAsync(loginViewModel.Email);

            if (userId is null)
                return OperationResult.Failure("The username or password is incorrect.");

            OperationResult signInResult = await _authService.LoginAsync(_mapper.Map<LoginDto>(loginViewModel));

            if (signInResult.Succeeded)
                return OperationResult.Success();
            else
                return OperationResult.Failure(signInResult.Errors.ToArray());
        }

        public async Task<OperationResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return OperationResult.Success();
        }
    }
}
