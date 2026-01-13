using Microsoft.AspNetCore.Mvc;

using PW.Web.Areas.Admin.Features.Categories.Services;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Controllers;

public class CategoryController : BaseAdminController
{
    private readonly ICategoryOrchestrator _categoryOrchestrator;
    public CategoryController(ICategoryOrchestrator categoryOrchestrator)
    {
        _categoryOrchestrator = categoryOrchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _categoryOrchestrator.PrepareCategoryListViewModelAsync();

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync("An error occurred while loading data.");
            return View(new CategoryListViewModel());
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var result = await _categoryOrchestrator.PrepareCreateViewModelAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateViewModel categoryCreateViewModel)
    {
        return await HandleFormAsync(
            viewModel: categoryCreateViewModel,
            workAction: () => _categoryOrchestrator.CreateCategoryAsync(categoryCreateViewModel),
            reloadAction: () => _categoryOrchestrator.PrepareCreateViewModelAsync(categoryCreateViewModel),
            successMessage: "Category created successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return RedirectToAction(nameof(Index));

        var result = await _categoryOrchestrator.PrepareEditViewModelAsync(id);

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryEditViewModel categoryEditViewModel)
    {
        return await HandleFormAsync(
            viewModel: categoryEditViewModel,
            workAction: () => _categoryOrchestrator.UpdateCategoryAsync(categoryEditViewModel),
            reloadAction: () => _categoryOrchestrator.PrepareEditViewModelAsync(categoryEditViewModel.Id, categoryEditViewModel),
            successMessage: "Category updated successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        return await HandleDeleteAsync(
            deleteAction: () => _categoryOrchestrator.DeleteCategoryAsync(id),
            successMessage: "Category deleted successfully."
        );
    }

}
