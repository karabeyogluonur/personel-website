using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Repositories;
using PW.Application.Models;
using PW.Application.Models.Dtos.Content;
using PW.Domain.Entities;

namespace PW.Services.Content;

public class CategoryService : ICategoryService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IRepository<Category> _categoryRepository;

   public CategoryService(IUnitOfWork unitOfWork)
   {
      _unitOfWork = unitOfWork;
      _categoryRepository = _unitOfWork.GetRepository<Category>();
   }

   public async Task<IList<CategorySummaryDto>> GetAllCategoriesAsync()
   {
      IList<Category> categories = await _categoryRepository.GetAllAsync(
          predicate: category => !category.IsDeleted,
          orderBy: query => query.OrderByDescending(category => category.CreatedAt)
      );

      IList<CategorySummaryDto> categorySummaryDtos = categories.Select(category => new CategorySummaryDto
      {
         Id = category.Id,
         IsActive = category.IsActive,
         CreatedAt = category.CreatedAt,
         Name = category.Name,
         Description = category.Description,
      }).ToList();

      return categorySummaryDtos;
   }

   public async Task<CategoryDetailDto?> GetCategoryByIdAsync(int categoryId)
   {
      if (categoryId <= 0) return null;

      Category category = await _categoryRepository.GetFirstOrDefaultAsync(
          predicate: category => category.Id == categoryId,
          include: source => source.Include(category => category.Translations)
      );

      if (category is null) return null;

      return new CategoryDetailDto
      {
         Id = category.Id,
         IsActive = category.IsActive,
         Name = category.Name,
         Description = category.Description,
         CreatedAt = category.CreatedAt,
         DeletedAt = category.DeletedAt,
         IsDeleted = category.IsDeleted,
         UpdatedAt = category.UpdatedAt,
         Translations = category.Translations.Select(translation => new CategoryTranslationDto
         {
            LanguageId = translation.LanguageId,
            Name = translation.Name,
            Description = translation.Description,
         }).ToList()
      };
   }

   public async Task<OperationResult> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
   {
      if (categoryCreateDto == null)
         throw new ArgumentNullException(nameof(categoryCreateDto));

      Category category = new Category
      {
         Name = categoryCreateDto.Name,
         Description = categoryCreateDto.Description,
         IsActive = categoryCreateDto.IsActive,
         CreatedAt = DateTime.UtcNow,
         IsDeleted = false,
         Translations = new List<CategoryTranslation>()
      };

      category.Translations.SyncTranslations(
          translationDtos: categoryCreateDto.Translations,
          isEmptyPredicate: (CategoryTranslationDto translationDto) =>
              string.IsNullOrWhiteSpace(translationDto.Name) &&
              string.IsNullOrWhiteSpace(translationDto.Description),
          mapAction: (CategoryTranslation translation, CategoryTranslationDto translationDto) =>
          {
             translation.Name = string.IsNullOrWhiteSpace(translationDto.Name) ? null : translationDto.Name;
             translation.Description = string.IsNullOrWhiteSpace(translationDto.Description) ? null : translationDto.Description;
          }
      );

      await _categoryRepository.InsertAsync(category);
      await _unitOfWork.CommitAsync();
      return OperationResult.Success();
   }

   public async Task<OperationResult> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
   {
      if (categoryUpdateDto == null)
         throw new ArgumentNullException(nameof(categoryUpdateDto));

      Category category = await _categoryRepository.GetFirstOrDefaultAsync(
          predicate: category => category.Id == categoryUpdateDto.Id,
          include: source => source.Include(category => category.Translations),
          disableTracking: false
      );

      if (category is null)
         return OperationResult.Failure("Category not found.", OperationErrorType.NotFound);

      category.Name = categoryUpdateDto.Name;
      category.Description = categoryUpdateDto.Description;
      category.IsActive = categoryUpdateDto.IsActive;
      category.Translations.SyncTranslations(
          translationDtos: categoryUpdateDto.Translations,

          isEmptyPredicate: (CategoryTranslationDto translationDto) =>
              string.IsNullOrWhiteSpace(translationDto.Name) &&
              string.IsNullOrWhiteSpace(translationDto.Description),

          mapAction: (CategoryTranslation translation, CategoryTranslationDto translationDto) =>
          {
             translation.Name = string.IsNullOrWhiteSpace(translationDto.Name) ? null : translationDto.Name;
             translation.Description = string.IsNullOrWhiteSpace(translationDto.Description) ? null : translationDto.Description;
          }
      );

      await _unitOfWork.CommitAsync();

      return OperationResult.Success();
   }

   public async Task<OperationResult> DeleteCategoryAsync(int categoryId)
   {
      Category category = await _categoryRepository.GetFirstOrDefaultAsync(
          predicate: category => category.Id == categoryId,
          include: source => source.Include(category => category.Translations)
      );

      if (category == null)
         return OperationResult.Failure("Category not found.", OperationErrorType.NotFound);

      _categoryRepository.Delete(category);
      await _unitOfWork.CommitAsync();

      return OperationResult.Success();
   }
}
