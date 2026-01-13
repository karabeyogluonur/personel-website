using System.Linq.Expressions;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Configurations;
using PW.Application.Models.Dtos.Localization;
using PW.Domain.Configuration;
using PW.Domain.Interfaces;

namespace PW.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
   private readonly ISettingService _settingService;
   private readonly IStorageService _storageService;
   private readonly ILanguageService _languageService;

   public ConfigurationService(ISettingService settingService, IStorageService storageService, ILanguageService languageService)
   {
      _settingService = settingService;
      _storageService = storageService;
      _languageService = languageService;
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

      currentSettings.LightThemeLogoFileName = await ProcessFileAsync(
          updateDto.LightThemeLogoStream,
          StoragePaths.System_Generals,
          updateDto.LightThemeLogoFileName,
          currentSettings.LightThemeLogoFileName,
          updateDto.RemoveLightThemeLogo,
          "light-logo");

      currentSettings.DarkThemeLogoFileName = await ProcessFileAsync(
          updateDto.DarkThemeLogoStream,
          StoragePaths.System_Generals,
          updateDto.DarkThemeLogoFileName,
          currentSettings.DarkThemeLogoFileName,
          updateDto.RemoveDarkThemeLogo,
          "dark-logo");

      currentSettings.LightThemeFaviconFileName = await ProcessFileAsync(
          updateDto.LightThemeFaviconStream,
          StoragePaths.System_Generals,
          updateDto.LightThemeFaviconFileName,
          currentSettings.LightThemeFaviconFileName,
          updateDto.RemoveLightThemeFavicon,
          "light-favicon");

      currentSettings.DarkThemeFaviconFileName = await ProcessFileAsync(
          updateDto.DarkThemeFaviconStream,
          StoragePaths.System_Generals,
          updateDto.DarkThemeFaviconFileName,
          currentSettings.DarkThemeFaviconFileName,
          updateDto.RemoveDarkThemeFavicon,
          "dark-favicon");

      currentSettings.SiteTitle = updateDto.SiteTitle;

      await _settingService.SaveSettingsAsync(currentSettings);

      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();

      foreach (GeneralSettingsTranslationUpdateDto translationUpdate in updateDto.Translations)
      {
         LanguageLookupDto? language = languages.FirstOrDefault(language => language.Id == translationUpdate.LanguageId);
         string langCode = language?.Code.ToLowerInvariant() ?? "unknown";

         await ProcessLocalizedFileAsync<GeneralSettings>(
             setting => setting.LightThemeLogoFileName,
             translationUpdate.LightThemeLogoStream,
             StoragePaths.System_Generals,
             translationUpdate.LightThemeLogoFileName,
             translationUpdate.RemoveLightThemeLogo,
             translationUpdate.LanguageId,
             $"light-logo-{langCode}");

         await ProcessLocalizedFileAsync<GeneralSettings>(
             setting => setting.DarkThemeLogoFileName,
             translationUpdate.DarkThemeLogoStream,
             StoragePaths.System_Generals,
             translationUpdate.DarkThemeLogoFileName,
             translationUpdate.RemoveDarkThemeLogo,
             translationUpdate.LanguageId,
             $"dark-logo-{langCode}");

         await ProcessLocalizedFileAsync<GeneralSettings>(
             setting => setting.LightThemeFaviconFileName,
             translationUpdate.LightThemeFaviconStream,
             StoragePaths.System_Generals,
             translationUpdate.LightThemeFaviconFileName,
             translationUpdate.RemoveLightThemeFavicon,
             translationUpdate.LanguageId,
             $"light-favicon-{langCode}");

         await ProcessLocalizedFileAsync<GeneralSettings>(
             setting => setting.DarkThemeFaviconFileName,
             translationUpdate.DarkThemeFaviconStream,
             StoragePaths.System_Generals,
             translationUpdate.DarkThemeFaviconFileName,
             translationUpdate.RemoveDarkThemeFavicon,
             translationUpdate.LanguageId,
             $"dark-favicon-{langCode}");

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

      currentSettings.AvatarFileName = await ProcessFileAsync(
          updateDto.AvatarStream,
          StoragePaths.System_Profiles,
          updateDto.AvatarFileName,
          currentSettings.AvatarFileName,
          updateDto.RemoveAvatar,
          "avatar");

      currentSettings.CoverFileName = await ProcessFileAsync(
          updateDto.CoverStream,
          StoragePaths.System_Profiles,
          updateDto.CoverFileName,
          currentSettings.CoverFileName,
          updateDto.RemoveCover,
          "cover");

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

         await ProcessLocalizedFileAsync<ProfileSettings>(
             setting => setting.AvatarFileName,
             translationUpdate.AvatarStream,
             StoragePaths.System_Profiles,
             translationUpdate.AvatarFileName,
             translationUpdate.RemoveAvatar,
             translationUpdate.LanguageId,
             $"avatar-{langCode}");

         await ProcessLocalizedFileAsync<ProfileSettings>(
             setting => setting.CoverFileName,
             translationUpdate.CoverStream,
             StoragePaths.System_Profiles,
             translationUpdate.CoverFileName,
             translationUpdate.RemoveCover,
             translationUpdate.LanguageId,
             $"cover-{langCode}");

         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.FirstName, translationUpdate.FirstName, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.LastName, translationUpdate.LastName, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.JobTitle, translationUpdate.JobTitle, translationUpdate.LanguageId);
         await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(setting => setting.Biography, translationUpdate.Biography, translationUpdate.LanguageId);
      }

      return OperationResult.Success();
   }

   #region Helper Methods

   private async Task<string> ProcessFileAsync(Stream? fileStream, string storagePaths, string? fileName, string? currentDbFileName, bool isRemove, string slugName)
   {
      if (fileStream != null && !string.IsNullOrEmpty(fileName))
      {
         if (!string.IsNullOrEmpty(currentDbFileName))
            await _storageService.DeleteAsync(storagePaths.ToString(), currentDbFileName);

         return await _storageService.UploadAsync(
             fileStream: fileStream,
             fileName: fileName,
             folder: storagePaths,
             mode: FileNamingMode.Unique,
             customName: slugName
         );
      }

      if (isRemove && !string.IsNullOrEmpty(currentDbFileName))
      {
         await _storageService.DeleteAsync(storagePaths.ToString(), currentDbFileName);
         return string.Empty;
      }

      return currentDbFileName ?? string.Empty;
   }

   private async Task ProcessLocalizedFileAsync<TSettings>(Expression<Func<TSettings, string>> keySelector, Stream? fileStream, string storagePaths, string? fileName, bool isRemove, int languageId, string slugName) where TSettings : ISettings, new()
   {
      if (fileStream == null && !isRemove) return;

      string currentLocalizedFileName = await _settingService.GetLocalizedSettingValueAsync(keySelector, languageId);

      string newFileName = await ProcessFileAsync(fileStream, storagePaths, fileName, currentLocalizedFileName, isRemove, slugName);

      await _settingService.SaveLocalizedSettingValueAsync(keySelector, newFileName, languageId);
   }

   #endregion
}
