using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.Technology.Services;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;
using PW.Web.Extensions;

namespace PW.Web.Areas.Admin.Controllers
{
    public class TechnologyController : BaseAdminController
    {
        private readonly ITechnologyOrchestrator _technologyOrchestrator;
        private readonly INotificationService _notificationService;

        public TechnologyController(ITechnologyOrchestrator technologyOrchestrator, INotificationService notificationService)
        {
            _technologyOrchestrator = technologyOrchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OperationResult<TechnologyListViewModel> result = await _technologyOrchestrator.PrepareTechnologyListViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return View(new TechnologyListViewModel());
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OperationResult<TechnologyCreateViewModel> result = await _technologyOrchestrator.PrepareCreateViewModelAsync();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TechnologyCreateViewModel technologyCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                await _notificationService.ErrorNotificationAsync("Please check the form for errors.");

                OperationResult<TechnologyCreateViewModel> reloadResult = await _technologyOrchestrator.PrepareCreateViewModelAsync(technologyCreateViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _technologyOrchestrator.CreateTechnologyAsync(technologyCreateViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("Technology created successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not create technology.");

            OperationResult<TechnologyCreateViewModel> errorReloadResult = await _technologyOrchestrator.PrepareCreateViewModelAsync(technologyCreateViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            OperationResult<TechnologyEditViewModel> result = await _technologyOrchestrator.PrepareEditViewModelAsync(id);

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TechnologyEditViewModel technologyEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                await _notificationService.ErrorNotificationAsync("Please check the form for errors.");
                OperationResult<TechnologyEditViewModel> reloadResult = await _technologyOrchestrator.PrepareEditViewModelAsync(technologyEditViewModel.Id, technologyEditViewModel);

                return View(reloadResult.Data);
            }

            OperationResult result = await _technologyOrchestrator.UpdateTechnologyAsync(technologyEditViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("Technology updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not update technology.");

            OperationResult<TechnologyEditViewModel> errorReloadResult = await _technologyOrchestrator.PrepareEditViewModelAsync(technologyEditViewModel.Id, technologyEditViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            OperationResult result = await _technologyOrchestrator.DeleteTechnologyAsync(id);

            if (result.Succeeded)
                await _notificationService.SuccessNotificationAsync("Technology deleted successfully.");
            else
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault() ?? "Error deleting technology.");

            return RedirectToAction(nameof(Index));
        }
    }
}
