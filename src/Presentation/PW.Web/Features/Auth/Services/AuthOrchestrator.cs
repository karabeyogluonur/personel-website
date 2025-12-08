using AutoMapper;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services
{
    public class AuthOrchestrator : IAuthOrchestrator
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthOrchestrator(IAuthService authService, IMapper mapper, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;

        }

        public async Task<OperationResult> LoginAsync(LoginViewModel loginViewModel)
        {
            UserDto userDto = await _userService.GetUserByEmailAsync(loginViewModel.Email);

            if (userDto is null)
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
