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
            LanguageListViewModel viewModel = await _languageOrchestrator.PrepareLanguageListViewModelAsync();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LanguageCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult result = await _languageOrchestrator.CreateLanguageAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                return View(model);
            }

            await _notificationService.SuccessNotificationAsync("Language added successfully. Please restart the application.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            OperationResult<LanguageEditViewModel> result = await _languageOrchestrator.PrepareLanguageEditViewModelAsync(id);

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LanguageEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult result = await _languageOrchestrator.UpdateLanguageAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result);
                return View(model);
            }

            await _notificationService.SuccessNotificationAsync("Language updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string code)
        {
            OperationResult result = await _languageOrchestrator.DeleteLanguageAsync(code);

            if (!result.Succeeded)
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
            else
                await _notificationService.SuccessNotificationAsync("Language deleted successfully.");

            return RedirectToAction(nameof(Index));
        }
    }
}
