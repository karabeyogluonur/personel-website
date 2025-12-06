using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Services
{
    public interface IAuthOrchestrator
    {
        Task<OperationResult> LoginAsync(LoginViewModel model);
        Task<OperationResult> LogoutAsync();
    }
}
