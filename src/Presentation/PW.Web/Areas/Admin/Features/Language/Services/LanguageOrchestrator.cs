using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language.Services
{
    public class LanguageOrchestrator : ILanguageOrchestrator
    {
        private readonly ILanguageService _languageService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public LanguageOrchestrator(
            ILanguageService languageService,
            IStorageService storageService,
            IMapper mapper)
        {
            _languageService = languageService;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<OperationResult<LanguageListViewModel>> PrepareLanguageListViewModelAsync()
        {
            var languages = await _languageService.GetAllLanguagesAsync();

            var languageListViewModel = new LanguageListViewModel
            {
                Languages = _mapper.Map<List<LanguageListItemViewModel>>(languages)
            };

            return OperationResult<LanguageListViewModel>.Success(languageListViewModel);
        }

        public async Task<OperationResult<LanguageCreateViewModel>> PrepareCreateViewModelAsync(LanguageCreateViewModel? languageCreateViewModel = null)
        {
            if (languageCreateViewModel is not null)
                return OperationResult<LanguageCreateViewModel>.Success(languageCreateViewModel);

            languageCreateViewModel = new LanguageCreateViewModel();
            return OperationResult<LanguageCreateViewModel>.Success(languageCreateViewModel);
        }

        public async Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel languageCreateViewModel)
        {
            string? flagFileName = null;

            if (languageCreateViewModel.FlagImage != null)
            {
                flagFileName = await _storageService.UploadAsync(
                    file: languageCreateViewModel.FlagImage,
                    folder: StoragePaths.System_Flags,
                    mode: FileNamingMode.Specific,
                    customName: languageCreateViewModel.Code
                );
            }

            var language = _mapper.Map<Domain.Entities.Language>(languageCreateViewModel);
            language.FlagImageFileName = flagFileName ?? string.Empty;

            var result = await _languageService.InsertLanguageAsync(language);

            if (result.IsFailure && !string.IsNullOrEmpty(flagFileName))
                await _storageService.DeleteAsync(StoragePaths.System_Flags, flagFileName);

            return result;
        }

        public async Task<OperationResult<LanguageEditViewModel>> PrepareEditViewModelAsync(int languageId, LanguageEditViewModel? languageEditViewModel = null)
        {
            if (languageEditViewModel is not null)
                return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);

            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language is null)
                return OperationResult<LanguageEditViewModel>.Failure("Language not found.", OperationErrorType.NotFound);

            languageEditViewModel = _mapper.Map<LanguageEditViewModel>(language);
            languageEditViewModel.CurrentFlagFileName = language.FlagImageFileName;

            return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);
        }

        public async Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel)
        {
            var existingLanguage = await _languageService.GetLanguageByIdAsync(languageEditViewModel.Id);

            if (existingLanguage is null)
                return OperationResult.Failure("Language not found.", OperationErrorType.NotFound);

            string? finalFileName = existingLanguage.FlagImageFileName;

            if (languageEditViewModel.FlagImage != null)
            {
                if (!string.IsNullOrEmpty(existingLanguage.FlagImageFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Flags, existingLanguage.FlagImageFileName);

                finalFileName = await _storageService.UploadAsync(
                    file: languageEditViewModel.FlagImage,
                    folder: StoragePaths.System_Flags,
                    mode: FileNamingMode.Specific,
                    customName: languageEditViewModel.Code
                );
            }
            else if (!string.Equals(existingLanguage.Code, languageEditViewModel.Code, StringComparison.OrdinalIgnoreCase)
                     && !string.IsNullOrEmpty(existingLanguage.FlagImageFileName))
            {
                string oldExtension = Path.GetExtension(existingLanguage.FlagImageFileName);
                string renamedFileName = $"{languageEditViewModel.Code.ToLowerInvariant()}{oldExtension}";

                await _storageService.RenameAsync(StoragePaths.System_Flags, existingLanguage.FlagImageFileName, renamedFileName);
                finalFileName = renamedFileName;
            }

            _mapper.Map(languageEditViewModel, existingLanguage);
            existingLanguage.FlagImageFileName = finalFileName ?? string.Empty;

            return await _languageService.UpdateLanguageAsync(existingLanguage);
        }

        public async Task<OperationResult> DeleteLanguageAsync(int languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language is null)
                return OperationResult.Failure("Language not found.", OperationErrorType.NotFound);

            var result = await _languageService.DeleteLanguageAsync(language);

            if (result.Succeeded && !string.IsNullOrEmpty(language.FlagImageFileName))
                await _storageService.DeleteAsync(StoragePaths.System_Flags, language.FlagImageFileName);

            return result;
        }
    }
}
