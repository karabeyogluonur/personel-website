using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Domain.Configuration;
using PW.Domain.Entities;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services
{
    public class ProfileSettingsOrchestrator : IProfileSettingsOrchestrator
    {
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ProfileSettingsOrchestrator(
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

        public async Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync()
        {
            ProfileSettings settings = _settingService.LoadSettings<ProfileSettings>();
            ProfileSettingsViewModel model = _mapper.Map<ProfileSettingsViewModel>(settings);

            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
            model.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);

            foreach (Domain.Entities.Language lang in languages)
            {

                ProfileSettingsLocalizedViewModel locale = new ProfileSettingsLocalizedViewModel
                {
                    LanguageId = lang.Id,
                    LanguageCode = lang.Code,
                    FirstName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, lang.Id),
                    LastName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, lang.Id),
                    Biography = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, lang.Id),
                    JobTitle = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, lang.Id),
                    AvatarPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, lang.Id),
                    CoverPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, lang.Id)
                };
                model.Locales.Add(locale);
            }

            return OperationResult<ProfileSettingsViewModel>.Success(model);
        }

        public async Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel model)
        {
            ProfileSettings currentSettings = _settingService.LoadSettings<ProfileSettings>();

            currentSettings.AvatarFileName = await ProcessFileAsync(
                model.AvatarImage, currentSettings.AvatarFileName, model.RemoveAvatar, "avatar");

            currentSettings.CoverFileName = await ProcessFileAsync(
                model.CoverImage, currentSettings.CoverFileName, model.RemoveCover, "cover");

            currentSettings.FirstName = model.FirstName;
            currentSettings.LastName = model.LastName;
            currentSettings.Biography = model.Biography;
            currentSettings.JobTitle = model.JobTitle;

            await _settingService.SaveSettingsAsync(currentSettings);

            foreach (ProfileSettingsLocalizedViewModel locale in model.Locales)
            {
                string currentLocAvatar = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, locale.LanguageId);
                string newLocAvatar = await ProcessFileAsync(
                    locale.AvatarImage, currentLocAvatar, locale.RemoveAvatar, $"avatar-{locale.LanguageCode}");

                string currentLocCover = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, locale.LanguageId);
                string newLocCover = await ProcessFileAsync(
                    locale.CoverImage, currentLocCover, locale.RemoveCover, $"cover-{locale.LanguageCode}");

                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, locale.FirstName, locale.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, locale.LastName, locale.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, locale.Biography, locale.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, locale.JobTitle, locale.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, newLocAvatar, locale.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, newLocCover, locale.LanguageId);
            }

            return OperationResult.Success();
        }

        private async Task<string> ProcessFileAsync(IFormFile newFile, string currentFileName, bool isRemove, string namePrefix)
        {
            if (isRemove && !string.IsNullOrEmpty(currentFileName))
            {
                await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);
                return null;
            }

            if (newFile is not null)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);

                string ext = Path.GetExtension(newFile.FileName).ToLowerInvariant();
                string fileName = $"{namePrefix}-{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";

                await _storageService.UploadAsync(newFile, StoragePaths.System_Profiles, fileName);
                return fileName;
            }

            return currentFileName;
        }
    }
}
