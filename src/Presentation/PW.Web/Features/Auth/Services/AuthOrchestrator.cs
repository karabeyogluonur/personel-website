// PW.Web.Features.Auth.Services/AuthOrchestrator.cs
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services
{
    public class AuthOrchestrator : IAuthOrchestrator
    {
        private readonly IIdentityService _identityService;

        public AuthOrchestrator(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<OperationResult> LoginAsync(LoginViewModel model)
        {
            int? userId = await _identityService.FindByEmailAsync(model.Email);

            if (userId is null)
                return OperationResult.Failure("The username or password is incorrect.");

            OperationResult signInResult = await _identityService.CheckPasswordSignInAsync(userId.Value, model.Password);

            if (signInResult.Succeeded)
                return OperationResult.Success();
            else
                return OperationResult.Failure("The username or password is incorrect.");
        }

        public async Task<OperationResult> LogoutAsync()
        {
            await _identityService.SignOutAsync();
            return OperationResult.Success();
        }
    }
}
