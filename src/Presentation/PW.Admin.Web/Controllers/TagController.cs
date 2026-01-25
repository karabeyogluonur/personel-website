using Microsoft.AspNetCore.Mvc;

using PW.Admin.Web.Features.Tags.Services;
using PW.Admin.Web.Features.Tags.ViewModels;

namespace PW.Admin.Web.Controllers;

public class TagController : BaseAdminController
{
    private readonly ITagOrchestrator _tagOrchestrator;
    public TagController(ITagOrchestrator tagOrchestrator)
    {
        _tagOrchestrator = tagOrchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _tagOrchestrator.PrepareTagListViewModelAsync();

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync("An error occurred while loading data.");
            return View(new TagListViewModel());
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var result = await _tagOrchestrator.PrepareCreateViewModelAsync();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TagCreateViewModel tagCreateViewModel)
    {
        return await HandleFormAsync(
            viewModel: tagCreateViewModel,
            workAction: () => _tagOrchestrator.CreateTagAsync(tagCreateViewModel),
            reloadAction: () => _tagOrchestrator.PrepareCreateViewModelAsync(tagCreateViewModel),
            successMessage: "Tag created successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return RedirectToAction(nameof(Index));

        var result = await _tagOrchestrator.PrepareEditViewModelAsync(id);

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TagEditViewModel tagEditViewModel)
    {
        return await HandleFormAsync(
            viewModel: tagEditViewModel,
            workAction: () => _tagOrchestrator.UpdateTagAsync(tagEditViewModel),
            reloadAction: () => _tagOrchestrator.PrepareEditViewModelAsync(tagEditViewModel.Id, tagEditViewModel),
            successMessage: "Tag updated successfully.",
            redirectTo: nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        return await HandleDeleteAsync(
            deleteAction: () => _tagOrchestrator.DeleteTagAsync(id),
            successMessage: "Tag deleted successfully."
        );
    }

}
