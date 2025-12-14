using PW.Application.Common.Models;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services
{
    public interface IGeneralSettingsOrchestrator
    {
        Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync();
        Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel);
    }
}
