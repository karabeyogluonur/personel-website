using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Categories.ViewModels;

namespace PW.Admin.Web.Features.Categories.Services;

public interface ICategoryOrchestrator
{
   Task<OperationResult<CategoryListViewModel>> PrepareCategoryListViewModelAsync();
   Task<OperationResult<CategoryCreateViewModel>> PrepareCreateViewModelAsync(CategoryCreateViewModel? categoryCreateViewModel = null);
   Task<OperationResult> CreateCategoryAsync(CategoryCreateViewModel categoryCreateViewModel);
   Task<OperationResult<CategoryEditViewModel>> PrepareEditViewModelAsync(int categoryId, CategoryEditViewModel? categoryEditViewModel = null);
   Task<OperationResult> UpdateCategoryAsync(CategoryEditViewModel categoryEditViewModel);
   Task<OperationResult> DeleteCategoryAsync(int categoryId);
}
