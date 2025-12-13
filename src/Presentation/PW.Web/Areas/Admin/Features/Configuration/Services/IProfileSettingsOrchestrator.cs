using PW.Application.Common.Models;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services
{
    public interface IProfileSettingsOrchestrator
    {
        Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync();
        Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel model);
    }
}
