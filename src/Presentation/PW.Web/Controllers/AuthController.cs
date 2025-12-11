using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Constants;
using PW.Application.Common.Models;
using PW.Web.Extensions;
using PW.Web.Features.Auth.Services;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Controllers
{
    public class AuthController : BasePublicController
    {
        private readonly IAuthOrchestrator _authOrchestrator;

        public AuthController(IAuthOrchestrator authOrchestrator)
        {
            _authOrchestrator = authOrchestrator;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            OperationResult result = await _authOrchestrator.LoginAsync(loginViewModel);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            ModelState.AddErrors(result);
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity!.IsAuthenticated)
                await _authOrchestrator.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}
