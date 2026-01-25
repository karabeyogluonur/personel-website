using PW.Admin.Web.Features.Auth.ViewModels;
using PW.Application.Utilities.Results;

namespace PW.Admin.Web.Features.Auth.Services;

public interface IAuthOrchestrator
{
   Task<OperationResult> LoginAsync(LoginViewModel loginViewModel);
   Task LogoutAsync();
}
