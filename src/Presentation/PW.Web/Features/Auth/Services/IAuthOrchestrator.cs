using PW.Application.Models;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services;

public interface IAuthOrchestrator
{
    Task<OperationResult> LoginAsync(LoginViewModel loginViewModel);
    Task LogoutAsync();
}
