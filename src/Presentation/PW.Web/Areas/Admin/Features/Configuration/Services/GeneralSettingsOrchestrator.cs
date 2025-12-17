using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Domain.Configuration;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services
{
    public class GeneralSettingsOrchestrator : IGeneralSettingsOrchestrator
    {
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public GeneralSettingsOrchestrator(
            ISettingService settingService,
            ILanguageService languageService,
            IStorageService storageService,
            IMapper mapper)
        {
            _settingService = settingService;
            _languageService = languageService;
            _storageService = storageService;
            _mapper = mapper;
        }

        private async Task LoadFormReferenceDataAsync(GeneralSettingsViewModel generalSettingsViewModel)
        {
            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
            generalSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);
        }

        public async Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync(GeneralSettingsViewModel? generalSettingsViewModel = null)
        {
            if (generalSettingsViewModel != null)
            {
                await LoadFormReferenceDataAsync(generalSettingsViewModel);
                return OperationResult<GeneralSettingsViewModel>.Success(generalSettingsViewModel);
            }

            GeneralSettings generalSettings = _settingService.LoadSettings<GeneralSettings>();
            generalSettingsViewModel = _mapper.Map<GeneralSettingsViewModel>(generalSettings);

            await LoadFormReferenceDataAsync(generalSettingsViewModel);

            foreach (LanguageListItemViewModel language in generalSettingsViewModel.AvailableLanguages)
            {
                GeneralSettingsLocalizedViewModel localizedModel = new GeneralSettingsLocalizedViewModel
                {
                    LanguageId = language.Id,
                    LanguageCode = language.Code,
                    SiteTitle = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(s => s.SiteTitle, language.Id),
                    DarkThemeFaviconPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(s => s.DarkThemeFaviconFileName, language.Id),
                    DarkThemeLogoPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(s => s.DarkThemeLogoFileName, language.Id),
                    LightThemeFaviconPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(s => s.LightThemeFaviconFileName, language.Id),
                    LightThemeLogoPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(s => s.LightThemeLogoFileName, language.Id),
                };

                generalSettingsViewModel.Locales.Add(localizedModel);
            }

            return OperationResult<GeneralSettingsViewModel>.Success(generalSettingsViewModel);
        }

        public async Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel)
        {
            GeneralSettings currentSettings = _settingService.LoadSettings<GeneralSettings>();

            currentSettings.DarkThemeFaviconFileName = await ProcessFileAsync(
                generalSettingsViewModel.DarkThemeFaviconImage,
                currentSettings.DarkThemeFaviconFileName,
                generalSettingsViewModel.RemoveDarkThemeFavicon,
                "dark-favicon");

            currentSettings.LightThemeFaviconFileName = await ProcessFileAsync(
                generalSettingsViewModel.LightThemeFaviconImage,
                currentSettings.LightThemeFaviconFileName,
                generalSettingsViewModel.RemoveLightThemeFavicon,
                "light-favicon");

            currentSettings.DarkThemeLogoFileName = await ProcessFileAsync(
                generalSettingsViewModel.DarkThemeLogoImage,
                currentSettings.DarkThemeLogoFileName,
                generalSettingsViewModel.RemoveDarkThemeLogo,
                "dark-logo");

            currentSettings.LightThemeLogoFileName = await ProcessFileAsync(
                generalSettingsViewModel.LightThemeLogoImage,
                currentSettings.LightThemeLogoFileName,
                generalSettingsViewModel.RemoveLightThemeLogo,
                "light-logo");

            currentSettings.SiteTitle = generalSettingsViewModel.SiteTitle;

            await _settingService.SaveSettingsAsync(currentSettings);

            foreach (GeneralSettingsLocalizedViewModel localizedViewModel in generalSettingsViewModel.Locales)
            {
                await ProcessLocalizedSettingFileAsync(
                    s => s.DarkThemeFaviconFileName,
                    localizedViewModel.DarkThemeFaviconImage,
                    localizedViewModel.RemoveDarkThemeFavicon,
                    localizedViewModel.LanguageId,
                    $"dark-favicon-{localizedViewModel.LanguageCode}");

                await ProcessLocalizedSettingFileAsync(
                    s => s.LightThemeFaviconFileName,
                    localizedViewModel.LightThemeFaviconImage,
                    localizedViewModel.RemoveLightThemeFavicon,
                    localizedViewModel.LanguageId,
                    $"light-favicon-{localizedViewModel.LanguageCode}");

                await ProcessLocalizedSettingFileAsync(
                    s => s.DarkThemeLogoFileName,
                    localizedViewModel.DarkThemeLogoImage,
                    localizedViewModel.RemoveDarkThemeLogo,
                    localizedViewModel.LanguageId,
                    $"dark-logo-{localizedViewModel.LanguageCode}");

                await ProcessLocalizedSettingFileAsync(
                    s => s.LightThemeLogoFileName,
                    localizedViewModel.LightThemeLogoImage,
                    localizedViewModel.RemoveLightThemeLogo,
                    localizedViewModel.LanguageId,
                    $"light-logo-{localizedViewModel.LanguageCode}");

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    s => s.SiteTitle,
                    localizedViewModel.SiteTitle,
                    localizedViewModel.LanguageId);
            }

            return OperationResult.Success();
        }

        private async Task<string?> ProcessFileAsync(IFormFile? newFile, string? currentFileName, bool isRemove, string namePrefix)
        {
            if (newFile != null)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Generals, currentFileName);

                string fileExtension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
                string fileName = $"{namePrefix}-{Guid.NewGuid().ToString()[..8]}{fileExtension}";

                await _storageService.UploadAsync(newFile, StoragePaths.System_Generals, fileName);
                return fileName;
            }

            if (isRemove && !string.IsNullOrEmpty(currentFileName))
            {
                await _storageService.DeleteAsync(StoragePaths.System_Generals, currentFileName);
                return null;
            }

            return currentFileName;
        }

        private async Task ProcessLocalizedSettingFileAsync(
            System.Linq.Expressions.Expression<Func<GeneralSettings, string>> keySelector,
            IFormFile? newFile,
            bool isRemove,
            int languageId,
            string namePrefix)
        {
            string? currentLocalizedFileName = await _settingService.GetLocalizedSettingValueAsync(keySelector, languageId);

            if (newFile == null && !isRemove) return;

            string? newLocalizedFileName = await ProcessFileAsync(newFile, currentLocalizedFileName, isRemove, namePrefix);

            await _settingService.SaveLocalizedSettingValueAsync(keySelector, newLocalizedFileName ?? string.Empty, languageId);
        }
    }
}
