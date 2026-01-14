using AutoMapper;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Common;
using PW.Application.Models.Dtos.Content;
using PW.Application.Models.Dtos.Localization;
using PW.Web.Areas.Admin.Features.Common.Models;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technologies.Services;

public class TechnologyOrchestrator : ITechnologyOrchestrator
{
   private readonly ITechnologyService _technologyService;
   private readonly ILanguageService _languageService;
   private readonly IStorageService _storageService;
   private readonly IMapper _mapper;

   public TechnologyOrchestrator(ITechnologyService technologyService, ILanguageService languageService, IMapper mapper, IStorageService storageService)
   {
      _technologyService = technologyService;
      _languageService = languageService;
      _storageService = storageService;
      _mapper = mapper;
   }

   public async Task<OperationResult<TechnologyListViewModel>> PrepareTechnologyListViewModelAsync()
   {
      IList<TechnologySummaryDto> technologySummaryDtos = await _technologyService.GetAllTechnologiesAsync();

      List<TechnologyListItemViewModel> technologyListItemViewModels = _mapper.Map<List<TechnologyListItemViewModel>>(technologySummaryDtos);

      TechnologyListViewModel technologyListViewModel = new TechnologyListViewModel
      {
         Technologies = technologyListItemViewModels
      };

      return OperationResult<TechnologyListViewModel>.Success(technologyListViewModel);
   }

   public async Task<OperationResult<TechnologyCreateViewModel>> PrepareCreateViewModelAsync(TechnologyCreateViewModel? technologyCreateViewModel = null)
   {
      if (technologyCreateViewModel != null)
      {
         await LoadFormReferenceDataAsync(technologyCreateViewModel);
         return OperationResult<TechnologyCreateViewModel>.Success(technologyCreateViewModel);
      }

      technologyCreateViewModel = new TechnologyCreateViewModel();
      await LoadFormReferenceDataAsync(technologyCreateViewModel);

      foreach (LanguageLookupViewModel language in technologyCreateViewModel.AvailableLanguages)
      {
         technologyCreateViewModel.Translations.Add(new TechnologyTranslationViewModel
         {
            LanguageId = language.Id,
            LanguageCode = language.Code
         });
      }

      return OperationResult<TechnologyCreateViewModel>.Success(technologyCreateViewModel);
   }

   public async Task<OperationResult> CreateTechnologyAsync(TechnologyCreateViewModel technologyCreateViewModel)
   {
      if (technologyCreateViewModel == null)
         throw new ArgumentNullException(nameof(technologyCreateViewModel));

      TechnologyCreateDto technologyCreateDto = new TechnologyCreateDto
      {
         Name = technologyCreateViewModel.Name,
         Description = technologyCreateViewModel.Description,
         IsActive = technologyCreateViewModel.IsActive,
         Icon = new FileUploadDto(technologyCreateViewModel.IconImage?.OpenReadStream(), technologyCreateViewModel.IconImage?.FileName, false),
         Translations = technologyCreateViewModel.Translations.Select(translationVm => new TechnologyTranslationDto
         {
            LanguageId = translationVm.LanguageId,
            Name = translationVm.Name ?? string.Empty,
            Description = translationVm.Description
         }).ToList()
      };

      return await _technologyService.CreateTechnologyAsync(technologyCreateDto);
   }

   public async Task<OperationResult<TechnologyEditViewModel>> PrepareEditViewModelAsync(int technologyId, TechnologyEditViewModel? technologyEditViewModel = null)
   {
      if (technologyEditViewModel != null)
      {
         await LoadFormReferenceDataAsync(technologyEditViewModel);
         return OperationResult<TechnologyEditViewModel>.Success(technologyEditViewModel);
      }

      TechnologyDetailDto? technologyUpdateDto = await _technologyService.GetTechnologyByIdAsync(technologyId);

      if (technologyUpdateDto == null)
         return OperationResult<TechnologyEditViewModel>.Failure("Technology not found.", OperationErrorType.NotFound);

      technologyEditViewModel = _mapper.Map<TechnologyEditViewModel>(technologyUpdateDto);

      await LoadFormReferenceDataAsync(technologyEditViewModel);

      foreach (LanguageLookupViewModel language in technologyEditViewModel.AvailableLanguages)
      {
         TechnologyTranslationDto? existingTranslation = technologyUpdateDto.Translations
             .FirstOrDefault(technology => technology.LanguageId == language.Id);

         technologyEditViewModel.Translations.Add(new TechnologyTranslationViewModel
         {
            LanguageId = language.Id,
            LanguageCode = language.Code,
            Name = existingTranslation?.Name,
            Description = existingTranslation?.Description
         });
      }

      return OperationResult<TechnologyEditViewModel>.Success(technologyEditViewModel);
   }

   public async Task<OperationResult> UpdateTechnologyAsync(TechnologyEditViewModel technologyEditViewModel)
   {
      if (technologyEditViewModel == null)
         throw new ArgumentNullException(nameof(technologyEditViewModel));


      TechnologyUpdateDto technologyUpdateDto = new TechnologyUpdateDto
      {
         Id = technologyEditViewModel.Id,
         Name = technologyEditViewModel.Name,
         Description = technologyEditViewModel.Description,
         IsActive = technologyEditViewModel.IsActive,
         Icon = new FileUploadDto(technologyEditViewModel.IconImage?.OpenReadStream(), technologyEditViewModel.IconImage?.FileName, technologyEditViewModel.RemoveIconImage),
         Translations = technologyEditViewModel.Translations.Select(translationVm => new TechnologyTranslationDto
         {
            LanguageId = translationVm.LanguageId,
            Name = translationVm.Name ?? string.Empty,
            Description = translationVm.Description
         }).ToList()
      };

      return await _technologyService.UpdateTechnologyAsync(technologyUpdateDto);
   }

   public async Task<OperationResult> DeleteTechnologyAsync(int technologyId)
   {
      return await _technologyService.DeleteTechnologyAsync(technologyId);
   }

   private async Task LoadFormReferenceDataAsync(TechnologyFormViewModel technologyFormViewModel)
   {
      IList<LanguageLookupDto> publishedLanguages = await _languageService.GetLanguagesLookupAsync();
      technologyFormViewModel.AvailableLanguages = _mapper.Map<List<LanguageLookupViewModel>>(publishedLanguages);
   }

}
