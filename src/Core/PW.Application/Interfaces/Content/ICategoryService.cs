using PW.Application.Models;
using PW.Application.Models.Dtos.Content;

namespace PW.Application.Interfaces.Content;

public interface ICategoryService
{
   Task<CategoryDetailDto?> GetCategoryByIdAsync(int categoryId);
   Task<IList<CategorySummaryDto>> GetAllCategoriesAsync();
   Task<OperationResult> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
   Task<OperationResult> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
   Task<OperationResult> DeleteCategoryAsync(int categoryId);
}
