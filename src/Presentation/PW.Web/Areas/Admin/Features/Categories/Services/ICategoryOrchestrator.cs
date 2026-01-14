using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories.Services;

public interface ICategoryOrchestrator
{
   Task<OperationResult<CategoryListViewModel>> PrepareCategoryListViewModelAsync();
   Task<OperationResult<CategoryCreateViewModel>> PrepareCreateViewModelAsync(CategoryCreateViewModel? categoryCreateViewModel = null);
   Task<OperationResult> CreateCategoryAsync(CategoryCreateViewModel categoryCreateViewModel);
   Task<OperationResult<CategoryEditViewModel>> PrepareEditViewModelAsync(int categoryId, CategoryEditViewModel? categoryEditViewModel = null);
   Task<OperationResult> UpdateCategoryAsync(CategoryEditViewModel categoryEditViewModel);
   Task<OperationResult> DeleteCategoryAsync(int categoryId);
}
