using AutoMapper;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Models;
using PW.Application.Models.Dtos.Common;
using PW.Application.Models.Dtos.Configurations;
using PW.Application.Models.Dtos.Localization;
using PW.Web.Areas.Admin.Features.Common.Models;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configurations.Services;

public class GeneralSettingsOrchestrator : IGeneralSettingsOrchestrator
{
   private readonly IConfigurationService _configurationService;
   private readonly ILanguageService _languageService;
   private readonly IMapper _mapper;

   public GeneralSettingsOrchestrator(IConfigurationService configurationService, ILanguageService languageService, IMapper mapper)
   {
      _configurationService = configurationService;
      _languageService = languageService;
      _mapper = mapper;
   }

   public async Task<OperationResult<GeneralSettingsViewModel>> PrepareGeneralSettingsViewModelAsync(GeneralSettingsViewModel? generalSettingsViewModel = null)
   {
      if (generalSettingsViewModel != null)
      {
         await LoadFormReferenceDataAsync(generalSettingsViewModel);
         return OperationResult<GeneralSettingsViewModel>.Success(generalSettingsViewModel);
      }

      GeneralSettingsDto settingsDto = await _configurationService.GetGeneralSettingsAsync();

      generalSettingsViewModel = _mapper.Map<GeneralSettingsViewModel>(settingsDto);

      await LoadFormReferenceDataAsync(generalSettingsViewModel);

      foreach (LanguageLookupViewModel language in generalSettingsViewModel.AvailableLanguages)
      {
         GeneralSettingsTranslationViewModel? existingTranslation = generalSettingsViewModel.Translations
             .FirstOrDefault(translation => translation.LanguageId == language.Id);

         if (existingTranslation == null)
         {
            generalSettingsViewModel.Translations.Add(new GeneralSettingsTranslationViewModel
            {
               LanguageId = language.Id,
               LanguageCode = language.Code
            });
         }
         else
            existingTranslation.LanguageCode = language.Code;
      }

      return OperationResult<GeneralSettingsViewModel>.Success(generalSettingsViewModel);
   }

   public async Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsViewModel generalSettingsViewModel)
   {
      if (generalSettingsViewModel == null)
         throw new ArgumentNullException(nameof(generalSettingsViewModel));

      GeneralSettingsUpdateDto updateDto = new GeneralSettingsUpdateDto
      {
         SiteTitle = generalSettingsViewModel.SiteTitle,
         LightThemeLogo = new FileUploadDto(
            generalSettingsViewModel.LightThemeLogoImage?.OpenReadStream(),
            generalSettingsViewModel.LightThemeLogoImage?.FileName,
            generalSettingsViewModel.RemoveLightThemeLogo
        ),

         DarkThemeLogo = new FileUploadDto(
            generalSettingsViewModel.DarkThemeLogoImage?.OpenReadStream(),
            generalSettingsViewModel.DarkThemeLogoImage?.FileName,
            generalSettingsViewModel.RemoveDarkThemeLogo
        ),

         LightThemeFavicon = new FileUploadDto(
            generalSettingsViewModel.LightThemeFaviconImage?.OpenReadStream(),
            generalSettingsViewModel.LightThemeFaviconImage?.FileName,
            generalSettingsViewModel.RemoveLightThemeFavicon
        ),

         DarkThemeFavicon = new FileUploadDto(
            generalSettingsViewModel.DarkThemeFaviconImage?.OpenReadStream(),
            generalSettingsViewModel.DarkThemeFaviconImage?.FileName,
            generalSettingsViewModel.RemoveDarkThemeFavicon
        ),


         Translations = generalSettingsViewModel.Translations.Select(translation => new GeneralSettingsTranslationUpdateDto
         {
            LanguageId = translation.LanguageId,
            SiteTitle = translation.SiteTitle ?? string.Empty,

            LightThemeLogo = new FileUploadDto(
               translation.LightThemeLogoImage?.OpenReadStream(),
               translation.LightThemeLogoImage?.FileName,
               translation.RemoveLightThemeLogo
           ),

            DarkThemeLogo = new FileUploadDto(
               translation.DarkThemeLogoImage?.OpenReadStream(),
               translation.DarkThemeLogoImage?.FileName,
               translation.RemoveDarkThemeLogo
           ),

            LightThemeFavicon = new FileUploadDto(
               translation.LightThemeFaviconImage?.OpenReadStream(),
               translation.LightThemeFaviconImage?.FileName,
               translation.RemoveLightThemeFavicon
           ),

            DarkThemeFavicon = new FileUploadDto(
               translation.DarkThemeFaviconImage?.OpenReadStream(),
               translation.DarkThemeFaviconImage?.FileName,
               translation.RemoveDarkThemeFavicon
           )

         }).ToList()
      };

      return await _configurationService.UpdateGeneralSettingsAsync(updateDto);
   }

   private async Task LoadFormReferenceDataAsync(GeneralSettingsViewModel generalSettingsViewModel)
   {
      IList<LanguageLookupDto> publishedLanguages = await _languageService.GetLanguagesLookupAsync();
      generalSettingsViewModel.AvailableLanguages = _mapper.Map<List<LanguageLookupViewModel>>(publishedLanguages);
   }
}
