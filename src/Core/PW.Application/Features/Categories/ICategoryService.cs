using PW.Application.Features.Categories.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Categories;

public interface ICategoryService
{
   Task<CategoryDetailDto?> GetCategoryByIdAsync(int categoryId);
   Task<IList<CategorySummaryDto>> GetAllCategoriesAsync();
   Task<OperationResult> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
   Task<OperationResult> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
   Task<OperationResult> DeleteCategoryAsync(int categoryId);
}
