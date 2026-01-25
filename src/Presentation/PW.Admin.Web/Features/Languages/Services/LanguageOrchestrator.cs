using AutoMapper;
using PW.Application.Common.Dtos;
using PW.Application.Common.Enums;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Languages.ViewModels;

namespace PW.Admin.Web.Features.Languages.Services;

public class LanguageOrchestrator : ILanguageOrchestrator
{
   private readonly ILanguageService _languageService;
   private readonly IMapper _mapper;

   public LanguageOrchestrator(ILanguageService languageService, IMapper mapper)
   {
      _languageService = languageService;
      _mapper = mapper;
   }

   public async Task<OperationResult<LanguageListViewModel>> PrepareLanguageListViewModelAsync()
   {
      IList<LanguageSummaryDto> languageSummaryDtos = await _languageService.GetAllLanguagesAsync();

      List<LanguageListItemViewModel> languageListItemViewModels = _mapper.Map<List<LanguageListItemViewModel>>(languageSummaryDtos);

      LanguageListViewModel languageListViewModel = new LanguageListViewModel
      {
         Languages = languageListItemViewModels
      };

      return OperationResult<LanguageListViewModel>.Success(languageListViewModel);
   }

   public async Task<OperationResult<LanguageCreateViewModel>> PrepareCreateViewModelAsync(LanguageCreateViewModel? languageCreateViewModel = null)
   {
      if (languageCreateViewModel != null)
         return OperationResult<LanguageCreateViewModel>.Success(languageCreateViewModel);

      languageCreateViewModel = new LanguageCreateViewModel();
      return OperationResult<LanguageCreateViewModel>.Success(languageCreateViewModel);
   }

   public async Task<OperationResult<LanguageEditViewModel>> PrepareEditViewModelAsync(int languageId, LanguageEditViewModel? languageEditViewModel = null)
   {
      if (languageEditViewModel != null)
         return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);

      LanguageDetailDto? languageDetailDto = await _languageService.GetLanguageByIdAsync(languageId);

      if (languageDetailDto == null)
         return OperationResult<LanguageEditViewModel>.Failure("Language not found.", OperationErrorType.NotFound);

      languageEditViewModel = _mapper.Map<LanguageEditViewModel>(languageDetailDto);

      return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);
   }

   public async Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel languageCreateViewModel)
   {
      if (languageCreateViewModel == null)
         throw new ArgumentNullException(nameof(languageCreateViewModel));

      LanguageCreateDto languageCreateDto = new LanguageCreateDto
      {
         Name = languageCreateViewModel.Name,
         Code = languageCreateViewModel.Code,
         DisplayOrder = languageCreateViewModel.DisplayOrder,
         IsPublished = languageCreateViewModel.IsPublished,
         IsDefault = languageCreateViewModel.IsDefault,
         FlagImage = new FileUploadDto(languageCreateViewModel.FlagImage?.OpenReadStream(), languageCreateViewModel.FlagImage?.FileName, false),
      };

      return await _languageService.CreateLanguageAsync(languageCreateDto);
   }

   public async Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel)
   {
      if (languageEditViewModel == null)
         throw new ArgumentNullException(nameof(languageEditViewModel));

      LanguageUpdateDto languageSummaryDto = new LanguageUpdateDto
      {
         LanguageId = languageEditViewModel.Id,
         Name = languageEditViewModel.Name,
         Code = languageEditViewModel.Code,
         DisplayOrder = languageEditViewModel.DisplayOrder,
         IsPublished = languageEditViewModel.IsPublished,
         IsDefault = languageEditViewModel.IsDefault,
         FlagImage = new FileUploadDto(languageEditViewModel.FlagImage?.OpenReadStream(), languageEditViewModel.FlagImage?.FileName, languageEditViewModel.RemoveFlagImage),

      };

      return await _languageService.UpdateLanguageAsync(languageSummaryDto);
   }

   public async Task<OperationResult> DeleteLanguageAsync(int languageId)
   {
      return await _languageService.DeleteLanguageAsync(languageId);
   }
}
