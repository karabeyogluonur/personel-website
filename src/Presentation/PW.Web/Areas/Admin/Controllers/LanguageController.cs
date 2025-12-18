using Microsoft.AspNetCore.Mvc;
using PW.Web.Areas.Admin.Features.Language.Services;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Controllers
{
    public class LanguageController : BaseAdminController
    {
        private readonly ILanguageOrchestrator _languageOrchestrator;

        public LanguageController(ILanguageOrchestrator languageOrchestrator)
        {
            _languageOrchestrator = languageOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _languageOrchestrator.PrepareLanguageListViewModelAsync();

            if (result.IsFailure)
            {
                await _notificationService.ErrorNotificationAsync("An error occurred while loading data.");
                return View(new LanguageListViewModel());
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var result = await _languageOrchestrator.PrepareCreateViewModelAsync();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCreateViewModel model)
        {
            return await HandleFormAsync(
                viewModel: model,
                workAction: () => _languageOrchestrator.CreateLanguageAsync(model),
                reloadAction: () => _languageOrchestrator.PrepareCreateViewModelAsync(model),
                successMessage: "Language added successfully. Please restart the application.",
                redirectTo: nameof(Index)
            );
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction(nameof(Index));

            var result = await _languageOrchestrator.PrepareEditViewModelAsync(id);

            if (result.IsFailure)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LanguageEditViewModel model)
        {
            return await HandleFormAsync(
                viewModel: model,
                workAction: () => _languageOrchestrator.UpdateLanguageAsync(model),
                reloadAction: () => _languageOrchestrator.PrepareEditViewModelAsync(model.Id, model),
                successMessage: "Language updated successfully."
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await HandleDeleteAsync(
                deleteAction: () => _languageOrchestrator.DeleteLanguageAsync(id),
                successMessage: "Language deleted successfully."
            );
        }
    }
}
