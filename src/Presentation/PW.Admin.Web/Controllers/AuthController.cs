using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PW.Admin.Web.Features.Auth.Services;
using PW.Admin.Web.Features.Auth.ViewModels;

namespace PW.Admin.Web.Controllers;

[AllowAnonymous]
public class AuthController : BaseAdminController
{
   private readonly IAuthOrchestrator _authOrchestrator;

   public AuthController(IAuthOrchestrator authOrchestrator)
   {
      _authOrchestrator = authOrchestrator;
   }

   [HttpGet]
   public IActionResult Login()
   {
      if (User.Identity?.IsAuthenticated == true)
         return RedirectToAction("Index", "Home");

      var loginViewModel = new LoginViewModel
      {
         Email = "admin@pw.com",
         Password = "Pass123*"
      };

      return View(loginViewModel);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Login(LoginViewModel loginViewModel)
   {
      if (!ModelState.IsValid)
         return View(loginViewModel);

      var result = await _authOrchestrator.LoginAsync(loginViewModel);

      if (result.Succeeded)
         return RedirectToAction("Index", "Home");

      foreach (var error in result.Errors)
         ModelState.AddModelError(string.Empty, error.Message);

      return View(loginViewModel);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Logout()
   {
      if (User.Identity?.IsAuthenticated is true)
         await _authOrchestrator.LogoutAsync();

      return RedirectToAction(nameof(Login));
   }
}
