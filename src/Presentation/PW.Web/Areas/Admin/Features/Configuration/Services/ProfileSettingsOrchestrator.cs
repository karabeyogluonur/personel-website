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
            ProfileSettings profileSettings = _settingService.LoadSettings<ProfileSettings>();
            ProfileSettingsViewModel profileSettingsViewModel = _mapper.Map<ProfileSettingsViewModel>(profileSettings);

            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
            profileSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);

            foreach (Domain.Entities.Language language in languages)
            {

                ProfileSettingsLocalizedViewModel locale = new ProfileSettingsLocalizedViewModel
                {
                    LanguageId = language.Id,
                    LanguageCode = language.Code,
                    FirstName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, language.Id),
                    LastName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, language.Id),
                    Biography = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, language.Id),
                    JobTitle = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, language.Id),
                    AvatarPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, language.Id),
                    CoverPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, language.Id)
                };
                profileSettingsViewModel.Locales.Add(locale);
            }

            return OperationResult<ProfileSettingsViewModel>.Success(profileSettingsViewModel);
        }

        public async Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel)
        {
            ProfileSettings currentProfileSettings = _settingService.LoadSettings<ProfileSettings>();

            currentProfileSettings.AvatarFileName = await ProcessFileAsync(
                profileSettingsViewModel.AvatarImage, currentProfileSettings.AvatarFileName, profileSettingsViewModel.RemoveAvatar, "avatar");

            currentProfileSettings.CoverFileName = await ProcessFileAsync(
                profileSettingsViewModel.CoverImage, currentProfileSettings.CoverFileName, profileSettingsViewModel.RemoveCover, "cover");

            currentProfileSettings.FirstName = profileSettingsViewModel.FirstName;
            currentProfileSettings.LastName = profileSettingsViewModel.LastName;
            currentProfileSettings.Biography = profileSettingsViewModel.Biography;
            currentProfileSettings.JobTitle = profileSettingsViewModel.JobTitle;

            await _settingService.SaveSettingsAsync(currentProfileSettings);

            foreach (ProfileSettingsLocalizedViewModel profileSettingsLocalizedViewModel in profileSettingsViewModel.Locales)
            {
                string currentLocalizedAvatar = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, profileSettingsLocalizedViewModel.LanguageId);
                string newLocalizedAvatar = await ProcessFileAsync(
                    profileSettingsLocalizedViewModel.AvatarImage, currentLocalizedAvatar, profileSettingsLocalizedViewModel.RemoveAvatar, $"avatar-{profileSettingsLocalizedViewModel.LanguageCode}");

                string currentLocalizedCover = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, profileSettingsLocalizedViewModel.LanguageId);
                string newLocalizedCover = await ProcessFileAsync(
                    profileSettingsLocalizedViewModel.CoverImage, currentLocalizedCover, profileSettingsLocalizedViewModel.RemoveCover, $"cover-{profileSettingsLocalizedViewModel.LanguageCode}");

                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, profileSettingsLocalizedViewModel.FirstName, profileSettingsLocalizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, profileSettingsLocalizedViewModel.LastName, profileSettingsLocalizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, profileSettingsLocalizedViewModel.Biography, profileSettingsLocalizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, profileSettingsLocalizedViewModel.JobTitle, profileSettingsLocalizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, newLocalizedAvatar, profileSettingsLocalizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, newLocalizedCover, profileSettingsLocalizedViewModel.LanguageId);
            }

            return OperationResult.Success();
        }

        private async Task<string> ProcessFileAsync(IFormFile newFile, string currentFileName, bool isRemove, string namePrefix)
        {
            if (newFile is not null)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);

                string extension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
                string fileName = $"{namePrefix}-{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";

                await _storageService.UploadAsync(newFile, StoragePaths.System_Profiles, fileName);
                return fileName;
            }

            if (isRemove && !string.IsNullOrEmpty(currentFileName))
            {
                await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);
                return null;
            }

            return currentFileName;
        }
    }
}
