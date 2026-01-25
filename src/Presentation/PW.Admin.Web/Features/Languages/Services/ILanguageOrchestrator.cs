using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Languages.ViewModels;

namespace PW.Admin.Web.Features.Languages.Services;

public interface ILanguageOrchestrator
{
   Task<OperationResult<LanguageListViewModel>> PrepareLanguageListViewModelAsync();

   Task<OperationResult<LanguageCreateViewModel>> PrepareCreateViewModelAsync(LanguageCreateViewModel? languageCreateViewModel = null);

   Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel languageCreateViewModel);

   Task<OperationResult<LanguageEditViewModel>> PrepareEditViewModelAsync(int languageId, LanguageEditViewModel? languageEditViewModel = null);

   Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel);

   Task<OperationResult> DeleteLanguageAsync(int languageId);
}
