using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Configurations;
using PW.Application.Models.Dtos.Localization;
using PW.Domain.Configuration;

namespace PW.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
   private readonly ISettingService _settingService;
   private readonly ILanguageService _languageService;
   private readonly IFileProcessorService _fileProcessorService;

   public ConfigurationService(
       ISettingService settingService,
       ILanguageService languageService,
       IFileProcessorService fileProcessorService)
   {
      _settingService = settingService;
      _languageService = languageService;
      _fileProcessorService = fileProcessorService;
   }

   public async Task<GeneralSettingsDto> GetGeneralSettingsAsync()
   {
      GeneralSettings settings = _settingService.LoadSettings<GeneralSettings>();

      GeneralSettingsDto resultDto = new GeneralSettingsDto
      {
         SiteTitle = settings.SiteTitle,
         LightThemeLogoFileName = settings.LightThemeLogoFileName,
         DarkThemeLogoFileName = settings.DarkThemeLogoFileName,
         LightThemeFaviconFileName = settings.LightThemeFaviconFileName,
         DarkThemeFaviconFileName = settings.DarkThemeFaviconFileName
      };

      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();

      foreach (LanguageLookupDto language in languages)
      {
         GeneralSettingsTranslationDto translationDto = new GeneralSettingsTranslationDto
         {
            LanguageId = language.Id,
            SiteTitle = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.SiteTitle, language.Id),
            LightThemeLogoFileName = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.LightThemeLogoFileName, language.Id),
            DarkThemeLogoFileName = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.DarkThemeLogoFileName, language.Id),
            LightThemeFaviconFileName = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.LightThemeFaviconFileName, language.Id),
            DarkThemeFaviconFileName = await _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(setting => setting.DarkThemeFaviconFileName, language.Id)
         };

         resultDto.Translations.Add(translationDto);
      }

      return resultDto;
   }

   public async Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsUpdateDto updateDto)
   {
      GeneralSettings currentSettings = _settingService.LoadSettings<GeneralSettings>();

      currentSettings.LightThemeLogoFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.LightThemeLogo,
          currentSettings.LightThemeLogoFileName,
          StoragePaths.System_Generals,
          "light-logo",
          FileNamingMode.Specific);

      currentSettings.DarkThemeLogoFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.DarkThemeLogo,
          currentSettings.DarkThemeLogoFileName,
          StoragePaths.System_Generals,
          "dark-logo",
          FileNamingMode.Specific);

      currentSettings.LightThemeFaviconFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.LightThemeFavicon,
          currentSettings.LightThemeFaviconFileName,
          StoragePaths.System_Generals,
          "light-favicon",
          FileNamingMode.Specific);

      currentSettings.DarkThemeFaviconFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.DarkThemeFavicon,
          currentSettings.DarkThemeFaviconFileName,
          StoragePaths.System_Generals,
          "dark-favicon",
          FileNamingMode.Specific);

      currentSettings.SiteTitle = updateDto.SiteTitle;

      await _settingService.SaveSettingsAsync(currentSettings);

      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();

      foreach (GeneralSettingsTranslationUpdateDto translationUpdate in updateDto.Translations)
      {
         LanguageLookupDto? language = languages.FirstOrDefault(language => language.Id == translationUpdate.LanguageId);
         string langCode = language?.Code.ToLowerInvariant() ?? "unknown";

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.LightThemeLogo,
             folderPath: StoragePaths.System_Generals,
             mode: FileNamingMode.Specific,
             slugName: $"light-logo-{langCode}",
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.LightThemeLogoFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.LightThemeLogoFileName, newFileName, translationUpdate.LanguageId)
         );

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.DarkThemeLogo,
             folderPath: StoragePaths.System_Generals,
             mode: FileNamingMode.Specific,
             slugName: $"dark-logo-{langCode}",
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.DarkThemeLogoFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.DarkThemeLogoFileName, newFileName, translationUpdate.LanguageId)
         );

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.LightThemeFavicon,
             folderPath: StoragePaths.System_Generals,
             mode: FileNamingMode.Specific,
             slugName: $"light-favicon-{langCode}",
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.LightThemeFaviconFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.LightThemeFaviconFileName, newFileName, translationUpdate.LanguageId)
         );

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.DarkThemeFavicon,
             folderPath: StoragePaths.System_Generals,
             mode: FileNamingMode.Specific,
             slugName: $"dark-favicon-{langCode}",
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.DarkThemeFaviconFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
                 setting => setting.DarkThemeFaviconFileName, newFileName, translationUpdate.LanguageId)
         );

         await _settingService.SaveLocalizedSettingValueAsync<GeneralSettings, string>(
             setting => setting.SiteTitle,
             translationUpdate.SiteTitle,
             translationUpdate.LanguageId);
      }

      return OperationResult.Success();
   }

   public async Task<ProfileSettingsDto> GetProfileSettingsAsync()
   {
      ProfileSettings settings = _settingService.LoadSettings<ProfileSettings>();

      ProfileSettingsDto resultDto = new ProfileSettingsDto
      {
         FirstName = settings.FirstName,
         LastName = settings.LastName,
         JobTitle = settings.JobTitle,
         Biography = settings.Biography,
         AvatarFileName = settings.AvatarFileName,
         CoverFileName = settings.CoverFileName
      };

      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();

      foreach (LanguageLookupDto language in languages)
      {
         ProfileSettingsTranslationDto translationDto = new ProfileSettingsTranslationDto
         {
            LanguageId = language.Id,
            FirstName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.FirstName, language.Id),
            LastName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.LastName, language.Id),
            JobTitle = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.JobTitle, language.Id),
            Biography = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.Biography, language.Id),
            AvatarFileName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.AvatarFileName, language.Id),
            CoverFileName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.CoverFileName, language.Id)
         };

         resultDto.Translations.Add(translationDto);
      }

      return resultDto;
   }

   public async Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsUpdateDto updateDto)
   {
      ProfileSettings currentSettings = _settingService.LoadSettings<ProfileSettings>();

      currentSettings.AvatarFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.Avatar,
          currentSettings.AvatarFileName,
          StoragePaths.System_Profiles,
          "avatar",
          FileNamingMode.Specific);

      currentSettings.CoverFileName = await _fileProcessorService.HandleFileUpdateAsync(
          updateDto.Cover,
          currentSettings.CoverFileName,
          StoragePaths.System_Profiles,
          "cover",
          FileNamingMode.Specific);

      currentSettings.FirstName = updateDto.FirstName;
      currentSettings.LastName = updateDto.LastName;
      currentSettings.JobTitle = updateDto.JobTitle;
      currentSettings.Biography = updateDto.Biography;

      await _settingService.SaveSettingsAsync(currentSettings);

      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();

      foreach (ProfileSettingsTranslationUpdateDto translationUpdate in updateDto.Translations)
      {
         LanguageLookupDto? language = languages.FirstOrDefault(language => language.Id == translationUpdate.LanguageId);
         string langCode = language?.Code.ToLowerInvariant() ?? "unknown";

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.Avatar,
             folderPath: StoragePaths.System_Profiles,
             slugName: $"avatar-{langCode}",
             mode: FileNamingMode.Specific,
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(
                 setting => setting.AvatarFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(
                 setting => setting.AvatarFileName, newFileName, translationUpdate.LanguageId)
         );

         await _fileProcessorService.ProcessFileActionAsync(
             fileInput: translationUpdate.Cover,
             folderPath: StoragePaths.System_Profiles,
             mode: FileNamingMode.Specific,
             slugName: $"cover-{langCode}",
             getCurrentFileNameAction: () => _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(
                 setting => setting.CoverFileName, translationUpdate.LanguageId),
             saveNewFileNameAction: (newFileName) => _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(
                 setting => setting.CoverFileName, newFileName, translationUpdate.LanguageId)
         );

         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.FirstName, translationUpdate.FirstName, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.LastName, translationUpdate.LastName, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.JobTitle, translationUpdate.JobTitle, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.Biography, translationUpdate.Biography, translationUpdate.LanguageId);
      }

      return OperationResult.Success();
   }
}
