using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Features.Configuration.Services;

public interface IProfileSettingsOrchestrator
{
   Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync(ProfileSettingsViewModel? profileSettingsViewModel = null);

   Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel);
}
