using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.Configuration.Services;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Web.Extensions;

namespace PW.Web.Areas.Admin.Controllers
{
    public class GeneralSettingsController : BaseAdminController
    {
        private readonly IGeneralSettingsOrchestrator _generalSettingsOrchestrator;
        private readonly INotificationService _notificationService;

        public GeneralSettingsController(
            IGeneralSettingsOrchestrator generalSettingsOrchestrator,
            INotificationService notificationService)
        {
            _generalSettingsOrchestrator = generalSettingsOrchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OperationResult<GeneralSettingsViewModel> result = await _generalSettingsOrchestrator.PrepareGeneralSettingsViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction("Index", "Home");
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(GeneralSettingsViewModel generalSettingsViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<GeneralSettingsViewModel> reloadResult = await _generalSettingsOrchestrator.PrepareGeneralSettingsViewModelAsync(generalSettingsViewModel);
                return View(reloadResult.Data);
            }

            OperationResult operationResult = await _generalSettingsOrchestrator.UpdateGeneralSettingsAsync(generalSettingsViewModel);

            if (!operationResult.Succeeded)
            {
                ModelState.AddErrors(operationResult);

                OperationResult<GeneralSettingsViewModel> errorReloadResult = await _generalSettingsOrchestrator.PrepareGeneralSettingsViewModelAsync(generalSettingsViewModel);
                return View(errorReloadResult.Data);
            }

            await _notificationService.SuccessNotificationAsync("General settings updated successfully.");

            return RedirectToAction(nameof(Index));
        }
    }
}
