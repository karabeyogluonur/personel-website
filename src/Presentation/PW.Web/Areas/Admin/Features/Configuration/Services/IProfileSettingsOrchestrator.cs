using PW.Application.Utilities.Results;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services;

public interface IProfileSettingsOrchestrator
{
   Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync(ProfileSettingsViewModel? profileSettingsViewModel = null);

   Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel);
}
