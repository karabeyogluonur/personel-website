using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.Language.Services;
using PW.Web.Areas.Admin.Features.Language.ViewModels;
using PW.Web.Extensions;

namespace PW.Web.Areas.Admin.Controllers
{
    public class LanguageController : BaseAdminController
    {
        private readonly ILanguageOrchestrator _languageOrchestrator;
        private readonly INotificationService _notificationService;

        public LanguageController(
            ILanguageOrchestrator languageOrchestrator,
            INotificationService notificationService)
        {
            _languageOrchestrator = languageOrchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OperationResult<LanguageListViewModel> result = await _languageOrchestrator.PrepareLanguageListViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return View(new LanguageListViewModel());
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OperationResult<LanguageCreateViewModel> result = await _languageOrchestrator.PrepareCreateViewModelAsync();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCreateViewModel languageCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<LanguageCreateViewModel> reloadResult = await _languageOrchestrator.PrepareCreateViewModelAsync(languageCreateViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _languageOrchestrator.CreateLanguageAsync(languageCreateViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("Language added successfully. Please restart the application.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not create language.");

            OperationResult<LanguageCreateViewModel> errorReloadResult = await _languageOrchestrator.PrepareCreateViewModelAsync(languageCreateViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction(nameof(Index));

            OperationResult<LanguageEditViewModel> result = await _languageOrchestrator.PrepareEditViewModelAsync(id);

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LanguageEditViewModel languageEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<LanguageEditViewModel> reloadResult = await _languageOrchestrator.PrepareEditViewModelAsync(languageEditViewModel.Id, languageEditViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _languageOrchestrator.UpdateLanguageAsync(languageEditViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("Language updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not update language.");

            OperationResult<LanguageEditViewModel> errorReloadResult = await _languageOrchestrator.PrepareEditViewModelAsync(languageEditViewModel.Id, languageEditViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            OperationResult result = await _languageOrchestrator.DeleteLanguageAsync(id);

            if (result.Succeeded)
                await _notificationService.SuccessNotificationAsync("Language deleted successfully.");
            else
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault() ?? "Error deleting language.");

            return RedirectToAction(nameof(Index));
        }
    }
}
