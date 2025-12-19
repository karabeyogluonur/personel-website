using PW.Application.Common.Enums;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Repositories;
using PW.Application.Models;
using PW.Domain.Entities;

namespace PW.Services.Content
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<Category>();
        }
        public async Task<OperationResult> DeleteCategoryAsync(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            // Business Rule: Check if used in any Project?
            // if (technology.Projects.Any()) return OperationResult.Failure("Cannot delete used category.", OperationErrorType.BusinessRule);

            try
            {
                _categoryRepository.Delete(category);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to delete category.", OperationErrorType.Technical);
            }
        }

        public async Task<IList<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            if (categoryId <= 0) return null;

            return await _categoryRepository.FindAsync(categoryId);
        }

        public async Task<OperationResult> InsertCategoryAsync(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            bool categoryExists = await _categoryRepository.ExistsAsync(t => t.Name == category.Name);

            if (categoryExists)
                return OperationResult.Failure("Category name exists.", OperationErrorType.Conflict);

            try
            {
                await _categoryRepository.InsertAsync(category);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to create category.", OperationErrorType.Technical);
            }
        }

        public async Task<OperationResult> UpdateCategoryAsync(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            Category existingCategory = await _categoryRepository.FindAsync(category.Id);

            if (existingCategory is null)
                return OperationResult.Failure("Technology not found.", OperationErrorType.NotFound);

            try
            {
                _categoryRepository.Update(category);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to update category.", OperationErrorType.Technical);
            }
        }
    }
}
