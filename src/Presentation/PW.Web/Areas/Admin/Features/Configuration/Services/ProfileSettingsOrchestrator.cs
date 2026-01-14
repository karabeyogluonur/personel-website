using AutoMapper;
using PW.Application.Common.Dtos;
using PW.Application.Features.Configuration;
using PW.Application.Features.Configuration.Dtos;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Utilities.Results;
using PW.Web.Areas.Admin.Features.Common.Models;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Services;

public class ProfileSettingsOrchestrator : IProfileSettingsOrchestrator
{
   private readonly IConfigurationService _configurationService;
   private readonly ILanguageService _languageService;
   private readonly IMapper _mapper;

   public ProfileSettingsOrchestrator(
       IConfigurationService configurationService,
       ILanguageService languageService,
       IMapper mapper)
   {
      _configurationService = configurationService;
      _languageService = languageService;
      _mapper = mapper;
   }

   public async Task<OperationResult<ProfileSettingsViewModel>> PrepareProfileSettingsViewModelAsync(ProfileSettingsViewModel? profileSettingsViewModel = null)
   {
      if (profileSettingsViewModel != null)
      {
         await LoadFormReferenceDataAsync(profileSettingsViewModel);
         return OperationResult<ProfileSettingsViewModel>.Success(profileSettingsViewModel);
      }

      ProfileSettingsDto settingsDto = await _configurationService.GetProfileSettingsAsync();

      profileSettingsViewModel = _mapper.Map<ProfileSettingsViewModel>(settingsDto);

      await LoadFormReferenceDataAsync(profileSettingsViewModel);

      foreach (LanguageLookupViewModel language in profileSettingsViewModel.AvailableLanguages)
      {
         ProfileSettingsTranslationViewModel? existingTranslation = profileSettingsViewModel.Translations
             .FirstOrDefault(translation => translation.LanguageId == language.Id);

         if (existingTranslation == null)
         {
            profileSettingsViewModel.Translations.Add(new ProfileSettingsTranslationViewModel
            {
               LanguageId = language.Id,
               LanguageCode = language.Code
            });
         }
         else
            existingTranslation.LanguageCode = language.Code;
      }

      return OperationResult<ProfileSettingsViewModel>.Success(profileSettingsViewModel);
   }

   public async Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsViewModel profileSettingsViewModel)
   {
      if (profileSettingsViewModel == null)
         throw new ArgumentNullException(nameof(profileSettingsViewModel));

      ProfileSettingsUpdateDto updateDto = new ProfileSettingsUpdateDto
      {
         FirstName = profileSettingsViewModel.FirstName,
         LastName = profileSettingsViewModel.LastName,
         JobTitle = profileSettingsViewModel.JobTitle,
         Biography = profileSettingsViewModel.Biography,

         Avatar = new FileUploadDto(
            profileSettingsViewModel.AvatarImage?.OpenReadStream(),
            profileSettingsViewModel.AvatarImage?.FileName,
            profileSettingsViewModel.RemoveAvatar
        ),

         Cover = new FileUploadDto(
            profileSettingsViewModel.CoverImage?.OpenReadStream(),
            profileSettingsViewModel.CoverImage?.FileName,
            profileSettingsViewModel.RemoveCover
        ),

         Translations = profileSettingsViewModel.Translations.Select(translation => new ProfileSettingsTranslationUpdateDto
         {
            LanguageId = translation.LanguageId,
            FirstName = translation.FirstName ?? string.Empty,
            LastName = translation.LastName ?? string.Empty,
            JobTitle = translation.JobTitle ?? string.Empty,
            Biography = translation.Biography ?? string.Empty,

            Avatar = new FileUploadDto(
               translation.AvatarImage?.OpenReadStream(),
               translation.AvatarImage?.FileName,
               translation.RemoveAvatar
           ),

            Cover = new FileUploadDto(
               translation.CoverImage?.OpenReadStream(),
               translation.CoverImage?.FileName,
               translation.RemoveCover
           )

         }).ToList()
      };

      return await _configurationService.UpdateProfileSettingsAsync(updateDto);
   }

   private async Task LoadFormReferenceDataAsync(ProfileSettingsViewModel profileSettingsViewModel)
   {
      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();
      profileSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageLookupViewModel>>(languages);
   }
}
