using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Language.ViewModels;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Services
{
    public class TechnologyOrchestrator : ITechnologyOrchestrator
    {
        private readonly ITechnologyService _technologyService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public TechnologyOrchestrator(
            ITechnologyService technologyService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStorageService storageService,
            IMapper mapper)
        {
            _technologyService = technologyService;
            _languageService = languageService;
            _localizationService = localizationService;
            _storageService = storageService;
            _mapper = mapper;
        }

        private async Task LoadFormReferenceDataAsync(TechnologyFormViewModel technologyFormViewModel)
        {
            var languages = await _languageService.GetAllPublishedLanguagesAsync();
            technologyFormViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);
        }

        private async Task<string> UploadIconAsync(IFormFile file, string baseName)
        {
            return await _storageService.UploadAsync(
                file: file,
                folder: StoragePaths.System_Technologies,
                mode: FileNamingMode.Unique,
                customName: baseName
            );
        }

        public async Task<OperationResult<TechnologyListViewModel>> PrepareTechnologyListViewModelAsync()
        {
            var technologies = await _technologyService.GetAllTechnologiesAsync();

            var technologyListViewModel = new TechnologyListViewModel
            {
                Technologies = _mapper.Map<List<TechnologyListItemViewModel>>(technologies)
            };

            return OperationResult<TechnologyListViewModel>.Success(technologyListViewModel);
        }

        public async Task<OperationResult<TechnologyCreateViewModel>> PrepareCreateViewModelAsync(TechnologyCreateViewModel? technologyCreateViewModel = null)
        {
            if (technologyCreateViewModel is not null)
            {
                await LoadFormReferenceDataAsync(technologyCreateViewModel);
                return OperationResult<TechnologyCreateViewModel>.Success(technologyCreateViewModel);
            }

            technologyCreateViewModel = new TechnologyCreateViewModel();
            await LoadFormReferenceDataAsync(technologyCreateViewModel);

            foreach (var availableLanguage in technologyCreateViewModel.AvailableLanguages)
            {
                technologyCreateViewModel.Locales.Add(new TechnologyLocalizedViewModel
                {
                    LanguageId = availableLanguage.Id,
                    LanguageCode = availableLanguage.Code
                });
            }

            return OperationResult<TechnologyCreateViewModel>.Success(technologyCreateViewModel);
        }

        public async Task<OperationResult> CreateTechnologyAsync(TechnologyCreateViewModel technologyCreateViewModel)
        {
            if (technologyCreateViewModel is null)
                throw new ArgumentNullException(nameof(technologyCreateViewModel));

            string fileName = string.Empty;

            if (technologyCreateViewModel.IconImage is not null)
            {
                fileName = await UploadIconAsync(technologyCreateViewModel.IconImage, technologyCreateViewModel.Name);
            }

            var technology = _mapper.Map<Domain.Entities.Technology>(technologyCreateViewModel);
            technology.IconImageFileName = fileName;

            var result = await _technologyService.InsertTechnologyAsync(technology);

            if (result.IsFailure)
            {
                if (!string.IsNullOrEmpty(fileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Technologies, fileName);

                return result;
            }

            foreach (var locale in technologyCreateViewModel.Locales)
            {
                if (!string.IsNullOrWhiteSpace(locale.Name))
                    await _localizationService.SaveLocalizedValueAsync(technology, x => x.Name, locale.Name, locale.LanguageId);

                if (!string.IsNullOrWhiteSpace(locale.Description))
                    await _localizationService.SaveLocalizedValueAsync(technology, x => x.Description, locale.Description, locale.LanguageId);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult<TechnologyEditViewModel>> PrepareEditViewModelAsync(int technologyId, TechnologyEditViewModel? technologyEditViewModel = null)
        {
            if (technologyEditViewModel is not null)
            {
                await LoadFormReferenceDataAsync(technologyEditViewModel);
                return OperationResult<TechnologyEditViewModel>.Success(technologyEditViewModel);
            }

            var technology = await _technologyService.GetTechnologyByIdAsync(technologyId);

            if (technology is null)
                return OperationResult<TechnologyEditViewModel>.Failure("Technology not found.", OperationErrorType.NotFound);

            technologyEditViewModel = _mapper.Map<TechnologyEditViewModel>(technology);
            technologyEditViewModel.CurrentIconFileName = technology.IconImageFileName;

            await LoadFormReferenceDataAsync(technologyEditViewModel);

            var translations = await _localizationService.GetLocalizedPropertiesAsync(new List<int> { technology.Id }, nameof(Domain.Entities.Technology), null);

            foreach (var availableLanguage in technologyEditViewModel.AvailableLanguages)
            {
                string? nameTranslation = translations.FirstOrDefault(x => x.LanguageId == availableLanguage.Id && x.LocaleKey == nameof(Domain.Entities.Technology.Name))?.LocaleValue;
                string? descriptionTranslation = translations.FirstOrDefault(x => x.LanguageId == availableLanguage.Id && x.LocaleKey == nameof(Domain.Entities.Technology.Description))?.LocaleValue;

                technologyEditViewModel.Locales.Add(new TechnologyLocalizedViewModel
                {
                    LanguageId = availableLanguage.Id,
                    LanguageCode = availableLanguage.Code,
                    Name = nameTranslation,
                    Description = descriptionTranslation
                });
            }

            return OperationResult<TechnologyEditViewModel>.Success(technologyEditViewModel);
        }

        public async Task<OperationResult> UpdateTechnologyAsync(TechnologyEditViewModel technologyEditViewModel)
        {
            if (technologyEditViewModel is null)
                throw new ArgumentNullException(nameof(technologyEditViewModel));

            var technology = await _technologyService.GetTechnologyByIdAsync(technologyEditViewModel.Id);

            if (technology is null)
                return OperationResult.Failure("Technology record not found in database.", OperationErrorType.NotFound);

            if (technologyEditViewModel.IconImage is not null)
            {
                if (!string.IsNullOrEmpty(technology.IconImageFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Technologies, technology.IconImageFileName);

                technology.IconImageFileName = await UploadIconAsync(technologyEditViewModel.IconImage, technologyEditViewModel.Name);
            }

            _mapper.Map(technologyEditViewModel, technology);

            var updateResult = await _technologyService.UpdateTechnologyAsync(technology);

            if (updateResult.IsFailure)
                return updateResult;

            foreach (var locale in technologyEditViewModel.Locales)
            {
                await _localizationService.SaveLocalizedValueAsync(technology, x => x.Name, locale.Name, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(technology, x => x.Description, locale.Description, locale.LanguageId);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteTechnologyAsync(int technologyId)
        {
            var technology = await _technologyService.GetTechnologyByIdAsync(technologyId);

            if (technology is null)
                return OperationResult.Failure("Technology not found.", OperationErrorType.NotFound);

            var deleteResult = await _technologyService.DeleteTechnologyAsync(technology);

            if (deleteResult.Succeeded && !string.IsNullOrEmpty(technology.IconImageFileName))
                await _storageService.DeleteAsync(StoragePaths.System_Technologies, technology.IconImageFileName);

            return deleteResult;
        }
    }
}
