using AutoMapper;
using PW.Application.Common.Enums;
using PW.Application.Features.Categories;
using PW.Application.Features.Categories.Dtos;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Categories.ViewModels;
using PW.Admin.Web.Features.Common.Models;

namespace PW.Admin.Web.Features.Categories.Services;

public class CategoryOrchestrator : ICategoryOrchestrator
{
   private readonly ICategoryService _categoryService;
   private readonly ILanguageService _languageService;
   private readonly IMapper _mapper;

   public CategoryOrchestrator(ICategoryService categoryService, ILanguageService languageService, IMapper mapper)
   {
      _categoryService = categoryService;
      _languageService = languageService;
      _mapper = mapper;
   }

   public async Task<OperationResult<CategoryListViewModel>> PrepareCategoryListViewModelAsync()
   {
      IList<CategorySummaryDto> categorySummaryDtos = await _categoryService.GetAllCategoriesAsync();

      List<CategoryListItemViewModel> categoryListItemViewModels = _mapper.Map<List<CategoryListItemViewModel>>(categorySummaryDtos);

      CategoryListViewModel categoryListViewModel = new CategoryListViewModel
      {
         Categories = categoryListItemViewModels
      };

      return OperationResult<CategoryListViewModel>.Success(categoryListViewModel);
   }

   public async Task<OperationResult<CategoryCreateViewModel>> PrepareCreateViewModelAsync(CategoryCreateViewModel? categoryCreateViewModel = null)
   {
      if (categoryCreateViewModel != null)
      {
         await LoadFormReferenceDataAsync(categoryCreateViewModel);
         return OperationResult<CategoryCreateViewModel>.Success(categoryCreateViewModel);
      }
      categoryCreateViewModel = new CategoryCreateViewModel();
      await LoadFormReferenceDataAsync(categoryCreateViewModel);

      foreach (LanguageLookupViewModel language in categoryCreateViewModel.AvailableLanguages)
      {
         categoryCreateViewModel.Translations.Add(new CategoryTranslationViewModel
         {
            LanguageId = language.Id,
            LanguageCode = language.Code
         });
      }

      return OperationResult<CategoryCreateViewModel>.Success(categoryCreateViewModel);
   }

   public async Task<OperationResult<CategoryEditViewModel>> PrepareEditViewModelAsync(int categoryId, CategoryEditViewModel? categoryEditViewModel = null)
   {
      if (categoryEditViewModel != null)
      {
         await LoadFormReferenceDataAsync(categoryEditViewModel);
         return OperationResult<CategoryEditViewModel>.Success(categoryEditViewModel);
      }

      CategoryDetailDto? categoryUpdateDto = await _categoryService.GetCategoryByIdAsync(categoryId);

      if (categoryUpdateDto == null)
         return OperationResult<CategoryEditViewModel>.Failure("Category not found.", OperationErrorType.NotFound);

      categoryEditViewModel = _mapper.Map<CategoryEditViewModel>(categoryUpdateDto);

      await LoadFormReferenceDataAsync(categoryEditViewModel);

      foreach (LanguageLookupViewModel language in categoryEditViewModel.AvailableLanguages)
      {
         CategoryTranslationDto? existingTranslationDto = categoryUpdateDto.Translations
             .FirstOrDefault(translation => translation.LanguageId == language.Id);

         categoryEditViewModel.Translations.Add(new CategoryTranslationViewModel
         {
            LanguageId = language.Id,
            LanguageCode = language.Code,
            Name = existingTranslationDto?.Name,
            Description = existingTranslationDto?.Description,
         });
      }

      return OperationResult<CategoryEditViewModel>.Success(categoryEditViewModel);
   }

   public async Task<OperationResult> CreateCategoryAsync(CategoryCreateViewModel categoryCreateViewModel)
   {
      if (categoryCreateViewModel == null)
         throw new ArgumentNullException(nameof(categoryCreateViewModel));

      CategoryCreateDto categoryCreateDto = new CategoryCreateDto
      {
         Name = categoryCreateViewModel.Name,
         Description = categoryCreateViewModel.Description,
         IsActive = categoryCreateViewModel.IsActive,
         Translations = categoryCreateViewModel.Translations.Select(translationViewModel => new CategoryTranslationDto
         {
            LanguageId = translationViewModel.LanguageId,
            Name = translationViewModel.Name ?? string.Empty,
            Description = translationViewModel.Description,
         }).ToList()
      };

      return await _categoryService.CreateCategoryAsync(categoryCreateDto);
   }

   public async Task<OperationResult> UpdateCategoryAsync(CategoryEditViewModel categoryEditViewModel)
   {
      if (categoryEditViewModel == null)
         throw new ArgumentNullException(nameof(categoryEditViewModel));

      CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto
      {
         Id = categoryEditViewModel.Id,
         Name = categoryEditViewModel.Name,
         Description = categoryEditViewModel.Description,
         IsActive = categoryEditViewModel.IsActive,

         Translations = categoryEditViewModel.Translations.Select(translationViewModel => new CategoryTranslationDto
         {
            LanguageId = translationViewModel.LanguageId,
            Name = translationViewModel.Name ?? string.Empty,
            Description = translationViewModel.Description,
         }).ToList()
      };

      return await _categoryService.UpdateCategoryAsync(categoryUpdateDto);
   }

   public async Task<OperationResult> DeleteCategoryAsync(int categoryId)
   {
      return await _categoryService.DeleteCategoryAsync(categoryId);
   }

   private async Task LoadFormReferenceDataAsync(CategoryFormViewModel categoryFormViewModel)
   {
      IList<LanguageLookupDto> publishedLanguages = await _languageService.GetLanguagesLookupAsync();
      categoryFormViewModel.AvailableLanguages = _mapper.Map<List<LanguageLookupViewModel>>(publishedLanguages);
   }
}
