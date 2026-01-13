using Microsoft.AspNetCore.Mvc;

using PW.Web.Areas.Admin.Features.Technologies.Services;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;

namespace PW.Web.Areas.Admin.Controllers;

public class TechnologyController : BaseAdminController
{
    private readonly ITechnologyOrchestrator _technologyOrchestrator;

    public TechnologyController(ITechnologyOrchestrator technologyOrchestrator)
    {
        _technologyOrchestrator = technologyOrchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _technologyOrchestrator.PrepareTechnologyListViewModelAsync();

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync("An error occurred while loading data.");
            return View(new TechnologyListViewModel());
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var result = await _technologyOrchestrator.PrepareCreateViewModelAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TechnologyCreateViewModel technologyCreateViewModel)
    {
        return await HandleFormAsync(
            viewModel: technologyCreateViewModel,
            workAction: () => _technologyOrchestrator.CreateTechnologyAsync(technologyCreateViewModel),
            reloadAction: () => _technologyOrchestrator.PrepareCreateViewModelAsync(technologyCreateViewModel),
            successMessage: "Technology created successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return RedirectToAction(nameof(Index));

        var result = await _technologyOrchestrator.PrepareEditViewModelAsync(id);

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TechnologyEditViewModel technologyEditViewModel)
    {
        return await HandleFormAsync(
            viewModel: technologyEditViewModel,
            workAction: () => _technologyOrchestrator.UpdateTechnologyAsync(technologyEditViewModel),
            reloadAction: () => _technologyOrchestrator.PrepareEditViewModelAsync(technologyEditViewModel.Id, technologyEditViewModel),
            successMessage: "Technology updated successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        return await HandleDeleteAsync(
            deleteAction: () => _technologyOrchestrator.DeleteTechnologyAsync(id),
            successMessage: "Technology deleted successfully."
        );
    }
}
