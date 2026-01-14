using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Languages.ViewModels;

namespace PW.Web.Areas.Admin.Features.Languages.Services;

public interface ILanguageOrchestrator
{
   Task<OperationResult<LanguageListViewModel>> PrepareLanguageListViewModelAsync();

   Task<OperationResult<LanguageCreateViewModel>> PrepareCreateViewModelAsync(LanguageCreateViewModel? languageCreateViewModel = null);

   Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel languageCreateViewModel);

   Task<OperationResult<LanguageEditViewModel>> PrepareEditViewModelAsync(int languageId, LanguageEditViewModel? languageEditViewModel = null);

   Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel);

   Task<OperationResult> DeleteLanguageAsync(int languageId);
}
