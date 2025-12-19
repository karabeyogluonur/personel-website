using PW.Application.Models;
using PW.Domain.Entities;

namespace PW.Application.Interfaces.Content
{
    public interface ICategoryService
    {
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<IList<Category>> GetAllCategoriesAsync();
        Task<OperationResult> InsertCategoryAsync(Category category);
        Task<OperationResult> UpdateCategoryAsync(Category category);
        Task<OperationResult> DeleteCategoryAsync(Category category);
    }
}
