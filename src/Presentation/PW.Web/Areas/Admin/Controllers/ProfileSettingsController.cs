using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.Configuration.Services;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Web.Extensions;

namespace PW.Web.Areas.Admin.Controllers
{
    public class ProfileSettingsController : BaseAdminController
    {
        private readonly IProfileSettingsOrchestrator _orchestrator;
        private readonly INotificationService _notificationService;

        public ProfileSettingsController(
            IProfileSettingsOrchestrator orchestrator,
            INotificationService notificationService)
        {
            _orchestrator = orchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OperationResult<ProfileSettingsViewModel> result = await _orchestrator.PrepareProfileSettingsViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction("Index", "Home");
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadAuxiliaryData(model);
                return View(model);
            }

            OperationResult result = await _orchestrator.UpdateProfileSettingsAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                await ReloadAuxiliaryData(model);
                return View(model);
            }

            await _notificationService.SuccessNotificationAsync("Profile settings updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        private async Task ReloadAuxiliaryData(ProfileSettingsViewModel model)
        {
            var prepareResult = await _orchestrator.PrepareProfileSettingsViewModelAsync();
            if (prepareResult.Succeeded)
            {
                model.AvailableLanguages = prepareResult.Data.AvailableLanguages;
            }
        }
    }
}
