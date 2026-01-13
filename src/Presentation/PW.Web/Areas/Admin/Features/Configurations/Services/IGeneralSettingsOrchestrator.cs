using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configurations.Services;

public interface IGeneralSettingsOrchestrator
{
    Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync(GeneralSettingsViewModel? generalSettingsViewModel = null);

    Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel);
}
