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
        public async Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync()
        {
            GeneralSettings generalSettings = _settingService.LoadSettings<GeneralSettings>();
            GeneralSettingsViewModel generalSettingsViewModel = _mapper.Map<GeneralSettingsViewModel>(generalSettings);

            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
            generalSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);

            foreach (Domain.Entities.Language language in languages)
            {
                GeneralSettingsLocalizedViewModel profileSettingsLocalizedViewModel = new GeneralSettingsLocalizedViewModel
                {
                    LanguageId = language.Id,
                    LanguageCode = language.Code,
                    SiteTitle = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.SiteTitle, language.Id),
                    DarkThemeFaviconPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.DarkThemeFaviconFileName, language.Id),
                    DarkThemeLogoPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.DarkThemeLogoFileName, language.Id),
                    LightThemeFaviconPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.LightThemeFaviconFileName, language.Id),
                    LightThemeLogoPath = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.LightThemeLogoFileName, language.Id),
                };

                generalSettingsViewModel.Locales.Add(profileSettingsLocalizedViewModel);
            }

            return OperationResult<GeneralSettingsViewModel>.Success(generalSettingsViewModel);


        }

        public async Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel)
        {
            GeneralSettings currentGeneralSettings = _settingService.LoadSettings<GeneralSettings>();

            currentGeneralSettings.DarkThemeFaviconFileName = await ProcessFileAsync(generalSettingsViewModel.DarkThemeFaviconImage, currentGeneralSettings.DarkThemeFaviconFileName, generalSettingsViewModel.RemoveDarkThemeFavicon, "dark-favicon");

            currentGeneralSettings.LightThemeFaviconFileName = await ProcessFileAsync(generalSettingsViewModel.LightThemeFaviconImage, currentGeneralSettings.LightThemeFaviconFileName, generalSettingsViewModel.RemoveLightThemeFavicon, "light-favicon");

            currentGeneralSettings.DarkThemeLogoFileName = await ProcessFileAsync(generalSettingsViewModel.DarkThemeLogoImage, currentGeneralSettings.DarkThemeLogoFileName, generalSettingsViewModel.RemoveDarkThemeLogo, "dark-logo");

            currentGeneralSettings.LightThemeLogoFileName = await ProcessFileAsync(generalSettingsViewModel.LightThemeLogoImage, currentGeneralSettings.LightThemeLogoFileName, generalSettingsViewModel.RemoveLightThemeLogo, "light-logo");

            currentGeneralSettings.SiteTitle = generalSettingsViewModel.SiteTitle;

            await _settingService.SaveSettingsAsync(currentGeneralSettings);


            foreach (GeneralSettingsLocalizedViewModel generalSettingsLocalizedViewModel in generalSettingsViewModel.Locales)
            {
                string currentLocalizedDarkThemeFavicon = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                        generalSetting => generalSetting.DarkThemeFaviconFileName,
                        generalSettingsLocalizedViewModel.LanguageId);

                string newLocalizedDarkThemeFavicon = await ProcessFileAsync(
                        generalSettingsLocalizedViewModel.DarkThemeFaviconImage,
                        currentLocalizedDarkThemeFavicon,
                        generalSettingsLocalizedViewModel.RemoveDarkThemeFavicon,
                        $"dark-favicon-{generalSettingsLocalizedViewModel.LanguageCode}");


                string currentLocalizedLightThemeFavicon = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                        generalSetting => generalSetting.LightThemeFaviconFileName,
                        generalSettingsLocalizedViewModel.LanguageId);

                string newLocalizedLightThemeFavicon = await ProcessFileAsync(
                        generalSettingsLocalizedViewModel.LightThemeFaviconImage,
                        currentLocalizedLightThemeFavicon,
                        generalSettingsLocalizedViewModel.RemoveLightThemeFavicon,
                        $"light-favicon-{generalSettingsLocalizedViewModel.LanguageCode}");


                string currentLocalizedDarkThemeLogo = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                        generalSetting => generalSetting.DarkThemeLogoFileName,
                        generalSettingsLocalizedViewModel.LanguageId);

                string newLocalizedDarkThemeLogo = await ProcessFileAsync(
                        generalSettingsLocalizedViewModel.DarkThemeLogoImage,
                        currentLocalizedDarkThemeLogo,
                        generalSettingsLocalizedViewModel.RemoveDarkThemeLogo,
                        $"dark-logo-{generalSettingsLocalizedViewModel.LanguageCode}");

                string currentLocalizedLightThemeLogo = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                        generalSetting => generalSetting.LightThemeLogoFileName,
                        generalSettingsLocalizedViewModel.LanguageId);

                string newLocalizedLightThemeLogo = await ProcessFileAsync(
                        generalSettingsLocalizedViewModel.LightThemeLogoImage,
                        currentLocalizedLightThemeLogo,
                        generalSettingsLocalizedViewModel.RemoveLightThemeLogo,
                        $"light-logo-{generalSettingsLocalizedViewModel.LanguageCode}");

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    generalSetting => generalSetting.DarkThemeFaviconFileName,
                    newLocalizedDarkThemeFavicon,
                    generalSettingsLocalizedViewModel.LanguageId);

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    generalSetting => generalSetting.LightThemeFaviconFileName,
                    newLocalizedLightThemeFavicon,
                    generalSettingsLocalizedViewModel.LanguageId);

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    generalSetting => generalSetting.DarkThemeLogoFileName,
                    newLocalizedDarkThemeLogo,
                    generalSettingsLocalizedViewModel.LanguageId);

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    generalSetting => generalSetting.LightThemeLogoFileName,
                    newLocalizedLightThemeLogo,
                    generalSettingsLocalizedViewModel.LanguageId);

                await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                    generalSetting => generalSetting.SiteTitle,
                    generalSettingsLocalizedViewModel.SiteTitle,
                    generalSettingsLocalizedViewModel.LanguageId);
            }

            return OperationResult.Success();

        }

        private async Task<string> ProcessFileAsync(IFormFile newFile, string currentFileName, bool isRemove, string namePrefix)
        {
            if (newFile is not null)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Generals, currentFileName);

                string extension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
                string fileName = $"{namePrefix}-{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";

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
    }
}
