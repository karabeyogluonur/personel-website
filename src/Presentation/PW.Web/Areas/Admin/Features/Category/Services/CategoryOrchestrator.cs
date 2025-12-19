using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Domain.Entities;
using PW.Web.Areas.Admin.Features.Category.ViewModels;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Category.Services
{
    public class CategoryOrchestrator : ICategoryOrchestrator
    {
        private readonly ICategoryService _categoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public CategoryOrchestrator(ICategoryService categoryService, ILanguageService languageService, ILocalizationService localizationService, IStorageService storageService, IMapper mapper)
        {
            _categoryService = categoryService;
            _languageService = languageService;
            _localizationService = localizationService;
            _storageService = storageService;
            _mapper = mapper;
        }


        public async Task<OperationResult> CreateCategoryAsync(CategoryCreateViewModel categoryCreateViewModel)
        {
            if (categoryCreateViewModel is null)
                throw new ArgumentNullException(nameof(categoryCreateViewModel));

            Domain.Entities.Category category = _mapper.Map<Domain.Entities.Category>(categoryCreateViewModel);

            if (categoryCreateViewModel.CoverImage is not null)
                category.CoverImageFileName = await ProcessCoverImageFileAsync(categoryCreateViewModel.CoverImage, categoryCreateViewModel.Name, false, null);

            OperationResult operationInsertResult = await _categoryService.InsertCategoryAsync(category);

            if (operationInsertResult.IsFailure)
            {
                if (!string.IsNullOrEmpty(category.CoverImageFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Categories, category.CoverImageFileName);

                return operationInsertResult;
            }

            foreach (CategoryLocalizedViewModel locale in categoryCreateViewModel.Locales)
            {
                await _localizationService.SaveLocalizedValueAsync(category, category => category.Name, locale.Name, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(category, category => category.Description, locale.Description, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(category, category => category.CoverImageFileName, await ProcessCoverImageFileAsync(
                    locale.CoverImage, locale.Name ?? categoryCreateViewModel.Name, locale.RemoveCurrentCoverImage, locale.CurrentCoverImageFileName
                ), locale.LanguageId);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteCategoryAsync(int categoryId)
        {
            Domain.Entities.Category category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category is null)
                return OperationResult.Failure("Category not found.", OperationErrorType.NotFound);

            IList<LocalizedProperty> localizedProperties = await _localizationService.GetLocalizedPropertiesAsync(new List<int> { categoryId }, nameof(Domain.Entities.Category), null);
            string standarCoverImage = category.CoverImageFileName;


            OperationResult deleteOperationResult = await _categoryService.DeleteCategoryAsync(category);

            if (!deleteOperationResult.Succeeded)
                return deleteOperationResult;

            if (!string.IsNullOrEmpty(standarCoverImage))
                await _storageService.DeleteAsync(StoragePaths.System_Categories, standarCoverImage);

            foreach (var localizedCoverImage in localizedProperties.Where(localizedProperty => localizedProperty.LocaleKey == nameof(Domain.Entities.Category.CoverImageFileName)))
            {
                if (!string.IsNullOrEmpty(localizedCoverImage.LocaleValue))
                    await _storageService.DeleteAsync(StoragePaths.System_Categories, localizedCoverImage.LocaleValue);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult<CategoryListViewModel>> PrepareCategoryListViewModelAsync()
        {
            IList<Domain.Entities.Category> categories = await _categoryService.GetAllCategoriesAsync();

            CategoryListViewModel categoryListViewModel = new CategoryListViewModel
            {
                Categories = _mapper.Map<List<CategoryListItemViewModel>>(categories)
            };

            return OperationResult<CategoryListViewModel>.Success(categoryListViewModel);
        }

        public async Task<OperationResult<CategoryCreateViewModel>> PrepareCreateViewModelAsync(CategoryCreateViewModel? categoryCreateViewModel = null)
        {
            if (categoryCreateViewModel is not null)
            {
                await LoadFormReferenceDataAsync(categoryCreateViewModel);
                return OperationResult<CategoryCreateViewModel>.Success(categoryCreateViewModel);
            }

            categoryCreateViewModel = new CategoryCreateViewModel();
            await LoadFormReferenceDataAsync(categoryCreateViewModel);

            foreach (var availableLanguage in categoryCreateViewModel.AvailableLanguages)
            {
                categoryCreateViewModel.Locales.Add(new CategoryLocalizedViewModel
                {
                    LanguageId = availableLanguage.Id,
                    LanguageCode = availableLanguage.Code
                });
            }

            return OperationResult<CategoryCreateViewModel>.Success(categoryCreateViewModel);
        }

        public async Task<OperationResult<CategoryEditViewModel>> PrepareEditViewModelAsync(int categoryId, CategoryEditViewModel? categoryEditViewModel = null)
        {
            if (categoryEditViewModel is not null)
            {
                await LoadFormReferenceDataAsync(categoryEditViewModel);
                return OperationResult<CategoryEditViewModel>.Success(categoryEditViewModel);
            }

            Domain.Entities.Category category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category is null)
                return OperationResult<CategoryEditViewModel>.Failure("Category not found.", OperationErrorType.NotFound);

            categoryEditViewModel = _mapper.Map<CategoryEditViewModel>(category);
            categoryEditViewModel.CurrentCoverImageFileName = category.CoverImageFileName;

            await LoadFormReferenceDataAsync(categoryEditViewModel);

            IList<LocalizedProperty> localizedProperties = await _localizationService.GetLocalizedPropertiesAsync(new List<int> { category.Id }, nameof(Domain.Entities.Category), null);

            foreach (var avaibleLanguage in categoryEditViewModel.AvailableLanguages)
            {
                categoryEditViewModel.Locales.Add(new CategoryLocalizedViewModel
                {
                    LanguageId = avaibleLanguage.Id,
                    LanguageCode = avaibleLanguage.Code,
                    Description = localizedProperties.FirstOrDefault(localizedProperty => localizedProperty.LanguageId == avaibleLanguage.Id && localizedProperty.LocaleKey == nameof(Domain.Entities.Category.Description))?.LocaleValue,
                    Name = localizedProperties.FirstOrDefault(localizedProperty => localizedProperty.LanguageId == avaibleLanguage.Id && localizedProperty.LocaleKey == nameof(Domain.Entities.Category.Name))?.LocaleValue,
                    CurrentCoverImageFileName = localizedProperties.FirstOrDefault(localizedProperty => localizedProperty.LanguageId == avaibleLanguage.Id && localizedProperty.LocaleKey == nameof(Domain.Entities.Category.CoverImageFileName))?.LocaleValue,
                });
            }

            return OperationResult<CategoryEditViewModel>.Success(categoryEditViewModel);
        }

        public async Task<OperationResult> UpdateCategoryAsync(CategoryEditViewModel categoryEditViewModel)
        {
            if (categoryEditViewModel is null)
                throw new ArgumentNullException(nameof(categoryEditViewModel));

            Domain.Entities.Category category = await _categoryService.GetCategoryByIdAsync(categoryEditViewModel.Id);

            if (category is null)
                return OperationResult.Failure("Category record not found.", OperationErrorType.NotFound);

            category.CoverImageFileName = await ProcessCoverImageFileAsync(categoryEditViewModel.CoverImage, category.Name, categoryEditViewModel.RemoveCurrentCoverImage, categoryEditViewModel.CurrentCoverImageFileName);

            _mapper.Map(categoryEditViewModel, category);

            OperationResult categoryUpdateResult = await _categoryService.UpdateCategoryAsync(category);

            if (categoryUpdateResult.IsFailure)
                return categoryUpdateResult;

            foreach (var locale in categoryEditViewModel.Locales)
            {
                await _localizationService.SaveLocalizedValueAsync(category, category => category.Name, locale.Name, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(category, category => category.Description, locale.Description, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(category, category => category.CoverImageFileName, await ProcessCoverImageFileAsync(
                    locale.CoverImage, locale.Name ?? categoryEditViewModel.Name, locale.RemoveCurrentCoverImage, locale.CurrentCoverImageFileName
                ), locale.LanguageId);
            }

            return OperationResult.Success();

        }

        private async Task LoadFormReferenceDataAsync(CategoryFormViewModel categoryFormViewModel)
        {
            var languages = await _languageService.GetAllPublishedLanguagesAsync();
            categoryFormViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);
        }
        private async Task<string> ProcessCoverImageFileAsync(IFormFile coverImage, string baseName, bool isRemove, string currentFileName)
        {
            if (coverImage is not null)
            {
                string newFileName = await _storageService.UploadAsync(coverImage, StoragePaths.System_Categories, FileNamingMode.Unique, customName: baseName);

                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Categories, currentFileName);

                return newFileName;
            }

            if (isRemove && !string.IsNullOrEmpty(currentFileName))
            {
                await _storageService.DeleteAsync(StoragePaths.System_Categories, currentFileName);
                return null;
            }

            return currentFileName;
        }
    }
}
