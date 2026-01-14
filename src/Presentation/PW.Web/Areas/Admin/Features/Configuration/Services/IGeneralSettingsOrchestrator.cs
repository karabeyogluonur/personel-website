using PW.Application.Utilities.Results;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services;

public interface IGeneralSettingsOrchestrator
{
   Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync(GeneralSettingsViewModel? generalSettingsViewModel = null);

   Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel);
}
