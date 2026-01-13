using Microsoft.AspNetCore.Mvc;

using PW.Web.Areas.Admin.Features.Configurations.Services;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Controllers;

public class ProfileSettingsController : BaseAdminController
{
    private readonly IProfileSettingsOrchestrator _orchestrator;

    public ProfileSettingsController(IProfileSettingsOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _orchestrator.PrepareProfileSettingsViewModelAsync();

        if (result.IsFailure)
        {
            await _notificationService.ErrorNotificationAsync("An error occurred while loading profile settings.");
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProfileSettingsViewModel profileSettingsViewModel)
    {
        return await HandleFormAsync(
            viewModel: profileSettingsViewModel,
            workAction: () => _orchestrator.UpdateProfileSettingsAsync(profileSettingsViewModel),
            reloadAction: () => _orchestrator.PrepareProfileSettingsViewModelAsync(profileSettingsViewModel),
            successMessage: "Profile settings updated successfully.",
            redirectTo: nameof(Index)
        );
    }
}
