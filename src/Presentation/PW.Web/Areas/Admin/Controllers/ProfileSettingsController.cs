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
        public async Task<IActionResult> Index(ProfileSettingsViewModel profileSettingsViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<ProfileSettingsViewModel> reloadResult = await _orchestrator.PrepareProfileSettingsViewModelAsync(profileSettingsViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _orchestrator.UpdateProfileSettingsAsync(profileSettingsViewModel);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                OperationResult<ProfileSettingsViewModel> errorReloadResult = await _orchestrator.PrepareProfileSettingsViewModelAsync(profileSettingsViewModel);
                return View(errorReloadResult.Data);
            }

            await _notificationService.SuccessNotificationAsync("Profile settings updated successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
