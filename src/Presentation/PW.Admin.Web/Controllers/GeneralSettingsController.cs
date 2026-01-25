using Microsoft.AspNetCore.Mvc;
using PW.Admin.Web.Features.Configuration.Services;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Controllers;

public class GeneralSettingsController : BaseAdminController
{
   private readonly IGeneralSettingsOrchestrator _generalSettingsOrchestrator;

   public GeneralSettingsController(IGeneralSettingsOrchestrator generalSettingsOrchestrator)
   {
      _generalSettingsOrchestrator = generalSettingsOrchestrator;
   }

   [HttpGet]
   public async Task<IActionResult> Index()
   {
      var result = await _generalSettingsOrchestrator.PrepareGeneralSettingsViewModelAsync();

      if (result.IsFailure)
      {
         await _notificationService.ErrorNotificationAsync("An error occurred while loading settings.");
         return RedirectToAction("Index", "Home");
      }

      return View(result.Data);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Index(GeneralSettingsViewModel generalSettingsViewModel)
   {
      return await HandleFormAsync(
          viewModel: generalSettingsViewModel,
          workAction: () => _generalSettingsOrchestrator.UpdateGeneralSettingsAsync(generalSettingsViewModel),
          reloadAction: () => _generalSettingsOrchestrator.PrepareGeneralSettingsViewModelAsync(generalSettingsViewModel),
          successMessage: "General settings updated successfully.",
          redirectTo: nameof(Index)
      );
   }
}
