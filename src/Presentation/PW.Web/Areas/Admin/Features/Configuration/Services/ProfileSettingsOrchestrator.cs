using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Domain.Configuration;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Web.Areas.Admin.Features.Language.ViewModels;
using System.Linq.Expressions;

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

        private async Task LoadFormReferenceDataAsync(ProfileSettingsViewModel profileSettingsViewModel)
        {
            IList<Domain.Entities.Language> languages = await _languageService.GetAllPublishedLanguagesAsync();
            profileSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageListItemViewModel>>(languages);
        }

        public async Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync(ProfileSettingsViewModel? profileSettingsViewModel = null)
        {
            if (profileSettingsViewModel != null)
            {
                await LoadFormReferenceDataAsync(profileSettingsViewModel);
                return OperationResult<ProfileSettingsViewModel>.Success(profileSettingsViewModel);
            }

            ProfileSettings profileSettings = _settingService.LoadSettings<ProfileSettings>();
            profileSettingsViewModel = _mapper.Map<ProfileSettingsViewModel>(profileSettings);

            await LoadFormReferenceDataAsync(profileSettingsViewModel);

            foreach (LanguageListItemViewModel language in profileSettingsViewModel.AvailableLanguages)
            {
                ProfileSettingsLocalizedViewModel locale = new ProfileSettingsLocalizedViewModel
                {
                    LanguageId = language.Id,
                    LanguageCode = language.Code,
                    FirstName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, language.Id),
                    LastName = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, language.Id),
                    JobTitle = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, language.Id),
                    Biography = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, language.Id),
                    AvatarPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.AvatarFileName, language.Id),
                    CoverPath = await _settingService.GetLocalizedSettingValueAsync<ProfileSettings, string>(x => x.CoverFileName, language.Id)
                };

                profileSettingsViewModel.Locales.Add(locale);
            }

            return OperationResult<ProfileSettingsViewModel>.Success(profileSettingsViewModel);
        }

        public async Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel)
        {
            ProfileSettings currentSettings = _settingService.LoadSettings<ProfileSettings>();

            currentSettings.AvatarFileName = await ProcessFileAsync(
                profileSettingsViewModel.AvatarImage,
                currentSettings.AvatarFileName,
                profileSettingsViewModel.RemoveAvatar,
                "avatar");

            currentSettings.CoverFileName = await ProcessFileAsync(
                profileSettingsViewModel.CoverImage,
                currentSettings.CoverFileName,
                profileSettingsViewModel.RemoveCover,
                "cover");

            currentSettings.FirstName = profileSettingsViewModel.FirstName;
            currentSettings.LastName = profileSettingsViewModel.LastName;
            currentSettings.Biography = profileSettingsViewModel.Biography;
            currentSettings.JobTitle = profileSettingsViewModel.JobTitle;

            await _settingService.SaveSettingsAsync(currentSettings);

            foreach (ProfileSettingsLocalizedViewModel localizedViewModel in profileSettingsViewModel.Locales)
            {
                await ProcessLocalizedSettingFileAsync(
                    x => x.AvatarFileName,
                    localizedViewModel.AvatarImage,
                    localizedViewModel.RemoveAvatar,
                    localizedViewModel.LanguageId,
                    $"avatar-{localizedViewModel.LanguageCode}");

                await ProcessLocalizedSettingFileAsync(
                    x => x.CoverFileName,
                    localizedViewModel.CoverImage,
                    localizedViewModel.RemoveCover,
                    localizedViewModel.LanguageId,
                    $"cover-{localizedViewModel.LanguageCode}");

                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.FirstName, localizedViewModel.FirstName, localizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.LastName, localizedViewModel.LastName, localizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.Biography, localizedViewModel.Biography, localizedViewModel.LanguageId);
                await _settingService.SaveLocalizedSettingValueAsync<ProfileSettings, string>(x => x.JobTitle, localizedViewModel.JobTitle, localizedViewModel.LanguageId);
            }

            return OperationResult.Success();
        }

        private async Task<string?> ProcessFileAsync(IFormFile? newFile, string? currentFileName, bool isRemove, string namePrefix)
        {
            if (newFile != null)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);

                return await _storageService.UploadAsync(
                    file: newFile,
                    folder: StoragePaths.System_Profiles,
                    mode: FileNamingMode.Unique,
                    customName: namePrefix
                );
            }

            if (isRemove && !string.IsNullOrEmpty(currentFileName))
            {
                await _storageService.DeleteAsync(StoragePaths.System_Profiles, currentFileName);
                return null;
            }

            return currentFileName;
        }

        private async Task ProcessLocalizedSettingFileAsync(
            Expression<Func<ProfileSettings, string>> keySelector,
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
