using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
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
            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
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
            IList<Domain.Entities.Technology> technologies = await _technologyService.GetAllTechnologiesAsync();

            TechnologyListViewModel technologyListViewModel = new TechnologyListViewModel
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

            foreach (LanguageListItemViewModel availableLanguage in technologyCreateViewModel.AvailableLanguages)
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
            string fileName = await UploadIconAsync(technologyCreateViewModel.IconImage, technologyCreateViewModel.Name);

            Domain.Entities.Technology technology = _mapper.Map<Domain.Entities.Technology>(technologyCreateViewModel);
            technology.IconImageFileName = fileName;

            await _technologyService.InsertTechnologyAsync(technology);

            foreach (TechnologyLocalizedViewModel locale in technologyCreateViewModel.Locales)
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

            Domain.Entities.Technology? technology = await _technologyService.GetTechnologyByIdAsync(technologyId);

            if (technology is null)
                return OperationResult<TechnologyEditViewModel>.Failure("Technology not found.");

            technologyEditViewModel = _mapper.Map<TechnologyEditViewModel>(technology);
            technologyEditViewModel.CurrentIconFileName = technology.IconImageFileName;

            await LoadFormReferenceDataAsync(technologyEditViewModel);

            IList<Domain.Entities.LocalizedProperty> translations = await _localizationService.GetLocalizedPropertiesAsync(new List<int> { technology.Id }, nameof(Domain.Entities.Technology), null);

            foreach (LanguageListItemViewModel availableLanguage in technologyEditViewModel.AvailableLanguages)
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
            Domain.Entities.Technology? technology = await _technologyService.GetTechnologyByIdAsync(technologyEditViewModel.Id);

            if (technology is null)
                return OperationResult.Failure("Technology record not found in database.");

            if (technologyEditViewModel.IconImage != null)
            {
                if (!string.IsNullOrEmpty(technology.IconImageFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Technologies, technology.IconImageFileName);

                technology.IconImageFileName = await UploadIconAsync(technologyEditViewModel.IconImage, technologyEditViewModel.Name);
            }

            _mapper.Map(technologyEditViewModel, technology);

            await _technologyService.UpdateTechnologyAsync(technology);

            foreach (TechnologyLocalizedViewModel locale in technologyEditViewModel.Locales)
            {
                await _localizationService.SaveLocalizedValueAsync(technology, x => x.Name, locale.Name, locale.LanguageId);
                await _localizationService.SaveLocalizedValueAsync(technology, x => x.Description, locale.Description, locale.LanguageId);
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteTechnologyAsync(int technologyId)
        {
            Domain.Entities.Technology? technology = await _technologyService.GetTechnologyByIdAsync(technologyId);

            if (technology is null)
                return OperationResult.Failure("Technology not found.");

            await _technologyService.DeleteTechnologyAsync(technology);

            if (!string.IsNullOrEmpty(technology.IconImageFileName))
                await _storageService.DeleteAsync(StoragePaths.System_Technologies, technology.IconImageFileName);

            return OperationResult.Success();
        }
    }
}
