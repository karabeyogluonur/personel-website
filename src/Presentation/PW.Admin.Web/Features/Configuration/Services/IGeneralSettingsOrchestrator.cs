using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Features.Configuration.Services;

public interface IGeneralSettingsOrchestrator
{
   Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync(GeneralSettingsViewModel? generalSettingsViewModel = null);

   Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel);
}
