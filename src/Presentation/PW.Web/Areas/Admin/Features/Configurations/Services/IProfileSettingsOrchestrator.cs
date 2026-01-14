using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configurations.Services;

public interface IProfileSettingsOrchestrator
{
   Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync(ProfileSettingsViewModel? profileSettingsViewModel = null);

   Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel);
}
