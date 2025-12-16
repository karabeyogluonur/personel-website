using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
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
            IList<Domain.Entities.Language> languages = await _languageService.GetAllLanguagesAsync();

            LanguageListViewModel languageListViewModel = new LanguageListViewModel
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
            Domain.Entities.Language? existingLanguage = await _languageService.GetLanguageByCodeAsync(languageCreateViewModel.Code);

            if (existingLanguage is not null)
                return OperationResult.Failure("Language code already exists.");

            string? flagFileName = null;

            if (languageCreateViewModel.FlagImage != null)
            {
                string fileExtension = Path.GetExtension(languageCreateViewModel.FlagImage.FileName).ToLowerInvariant();
                string fileName = $"{languageCreateViewModel.Code.ToLowerInvariant()}{fileExtension}";

                await _storageService.UploadAsync(languageCreateViewModel.FlagImage, StoragePaths.System_Flags, fileName);

                flagFileName = fileName;
            }

            Domain.Entities.Language language = _mapper.Map<Domain.Entities.Language>(languageCreateViewModel);
            language.FlagImageFileName = flagFileName ?? string.Empty;

            await _languageService.InsertLanguageAsync(language);

            return OperationResult.Success();
        }

        public async Task<OperationResult<LanguageEditViewModel>> PrepareEditViewModelAsync(int languageId, LanguageEditViewModel? languageEditViewModel = null)
        {
            if (languageEditViewModel is not null)
                return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);

            Domain.Entities.Language? language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language is null)
                return OperationResult<LanguageEditViewModel>.Failure("Language not found.");

            languageEditViewModel = _mapper.Map<LanguageEditViewModel>(language);
            languageEditViewModel.CurrentFlagFileName = language.FlagImageFileName;

            return OperationResult<LanguageEditViewModel>.Success(languageEditViewModel);
        }

        public async Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel languageEditViewModel)
        {
            Domain.Entities.Language? existingLanguage = await _languageService.GetLanguageByIdAsync(languageEditViewModel.Id);

            if (existingLanguage is null)
                return OperationResult.Failure("Language not found.");

            if (!string.Equals(existingLanguage.Code, languageEditViewModel.Code, StringComparison.OrdinalIgnoreCase))
            {
                Domain.Entities.Language? duplicateCheck = await _languageService.GetLanguageByCodeAsync(languageEditViewModel.Code);
                if (duplicateCheck is not null)
                    return OperationResult.Failure("Language code already exists.");
            }

            string? finalFileName = existingLanguage.FlagImageFileName;

            if (languageEditViewModel.FlagImage != null)
            {
                if (!string.IsNullOrEmpty(existingLanguage.FlagImageFileName))
                    await _storageService.DeleteAsync(StoragePaths.System_Flags, existingLanguage.FlagImageFileName);

                string fileExtension = Path.GetExtension(languageEditViewModel.FlagImage.FileName).ToLowerInvariant();
                string newFileName = $"{languageEditViewModel.Code.ToLowerInvariant()}{fileExtension}";

                await _storageService.UploadAsync(languageEditViewModel.FlagImage, StoragePaths.System_Flags, newFileName);
                finalFileName = newFileName;
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

            await _languageService.UpdateLanguageAsync(existingLanguage);

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteLanguageAsync(int languageId)
        {
            Domain.Entities.Language? language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language is null)
                return OperationResult.Failure("Language not found.");

            if (language.IsDefault)
                return OperationResult.Failure("Cannot delete the default language.");

            await _languageService.DeleteLanguageAsync(language);

            if (!string.IsNullOrEmpty(language.FlagImageFileName))
                await _storageService.DeleteAsync(StoragePaths.System_Flags, language.FlagImageFileName);

            return OperationResult.Success();
        }
    }
}
