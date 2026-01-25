using Microsoft.AspNetCore.Mvc;
using PW.Admin.Web.Features.Configuration.Services;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Controllers;

public class ProfileSettingsController : BaseAdminController
{
   private readonly IProfileSettingsOrchestrator _orchestrator;

   public ProfileSettingsController(IProfileSettingsOrchestrator orchestrator)
   {
      _orchestrator = orchestrator;
   }

   [HttpGet]
   public async Task<IActionResult> Index()
   {
      var result = await _orchestrator.PrepareProfileSettingsViewModelAsync();

      if (result.IsFailure)
      {
         await _notificationService.ErrorNotificationAsync("An error occurred while loading profile settings.");
         return RedirectToAction("Index", "Home");
      }

      return View(result.Data);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Index(ProfileSettingsViewModel profileSettingsViewModel)
   {
      return await HandleFormAsync(
          viewModel: profileSettingsViewModel,
          workAction: () => _orchestrator.UpdateProfileSettingsAsync(profileSettingsViewModel),
          reloadAction: () => _orchestrator.PrepareProfileSettingsViewModelAsync(profileSettingsViewModel),
          successMessage: "Profile settings updated successfully.",
          redirectTo: nameof(Index)
      );
   }
}
