using PW.Application.Common.Models;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language.Services
{
    public interface ILanguageOrchestrator
    {
        Task<LanguageListViewModel> PrepareLanguageListViewModelAsync();
        Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel languageCreateViewModel);
        Task<OperationResult<LanguageEditViewModel>> PrepareLanguageEditViewModelAsync(int id);
        Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel);
        Task<OperationResult> DeleteLanguageAsync(string code);
    }
}
