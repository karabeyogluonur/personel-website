using AutoMapper;
using PW.Application.Common.Constants;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Storage;
using PW.Domain.Entities;
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

        public async Task<LanguageListViewModel> PrepareLanguageListViewModelAsync()
        {
            IList<Domain.Entities.Language> languages = await _languageService.GetAllLanguagesAsync();

            LanguageListViewModel model = new LanguageListViewModel
            {
                Languages = _mapper.Map<List<LanguageListItemViewModel>>(languages)
            };

            return model;
        }

        public async Task<OperationResult> CreateLanguageAsync(LanguageCreateViewModel model)
        {
            Domain.Entities.Language existingLanguage = await _languageService.GetLanguageByCodeAsync(model.Code);

            if (existingLanguage is not null)
                return OperationResult.Failure("Language code already exists.");

            string flagFileName = null;

            if (model.FlagImage != null)
            {
                string extension = Path.GetExtension(model.FlagImage.FileName).ToLowerInvariant();
                string fileName = $"{model.Code.ToLowerInvariant()}{extension}";

                await _storageService.UploadAsync(model.FlagImage, StoragePaths.System_Flags, fileName);

                flagFileName = fileName;
            }

            Domain.Entities.Language language = _mapper.Map<Domain.Entities.Language>(model);
            language.FlagImageFileName = flagFileName;

            await _languageService.InsertLanguageAsync(language);

            return OperationResult.Success();
        }

        public async Task<OperationResult<LanguageEditViewModel>> PrepareLanguageEditViewModelAsync(int id)
        {
            Domain.Entities.Language language = await _languageService.GetLanguageByIdAsync(id);

            if (language is null)
                return OperationResult<LanguageEditViewModel>.Failure("Language not found.");

            LanguageEditViewModel model = _mapper.Map<LanguageEditViewModel>(language);

            return OperationResult<LanguageEditViewModel>.Success(model);
        }

        public async Task<OperationResult> UpdateLanguageAsync(LanguageEditViewModel model)
        {
            Domain.Entities.Language existingLanguage = await _languageService.GetLanguageByIdAsync(model.Id);

            if (existingLanguage is null)
                return OperationResult.Failure("Language not found.");

            string oldFileName = existingLanguage.FlagImageFileName;
            string finalFileName = oldFileName;
            bool fileRenamed = false;
            string oldCode = existingLanguage.Code;
            bool codeChanged = !string.Equals(oldCode, model.Code, StringComparison.OrdinalIgnoreCase);

            try
            {
                if (model.FlagImage != null)
                {
                    if (!string.IsNullOrEmpty(oldFileName))
                        await _storageService.DeleteAsync(StoragePaths.System_Flags, oldFileName);

                    string ext = Path.GetExtension(model.FlagImage.FileName).ToLowerInvariant();
                    string newFile = $"{model.Code.ToLowerInvariant()}{ext}";

                    await _storageService.UploadAsync(model.FlagImage, StoragePaths.System_Flags, newFile);
                    finalFileName = newFile;
                }
                else if (codeChanged && !string.IsNullOrEmpty(oldFileName))
                {
                    string ext = Path.GetExtension(oldFileName);
                    string newRenamedFile = $"{model.Code.ToLowerInvariant()}{ext}";

                    await _storageService.RenameAsync(StoragePaths.System_Flags, oldFileName, newRenamedFile);

                    finalFileName = newRenamedFile;
                    fileRenamed = true;
                }

                _mapper.Map(model, existingLanguage);
                existingLanguage.FlagImageFileName = finalFileName;

                await _languageService.UpdateLanguageAsync(existingLanguage);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                if (fileRenamed)
                    await _storageService.RenameAsync(StoragePaths.System_Flags, finalFileName, oldFileName);

                return OperationResult.Failure($"Update failed: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteLanguageAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                return OperationResult.Failure("Code cannot be empty.");

            Domain.Entities.Language language = await _languageService.GetLanguageByCodeAsync(code);

            if (language is null)
                return OperationResult.Failure("Language not found.");

            if (language.IsDefault)
                return OperationResult.Failure("Cannot delete default language.");

            string fileNameToDelete = language.FlagImageFileName;

            try
            {
                await _languageService.DeleteLanguageAsync(language);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Database deletion failed: {ex.Message}");
            }


            try
            {
                await _storageService.DeleteAsync(StoragePaths.System_Flags, fileNameToDelete);
            }
            catch
            {
            }

            return OperationResult.Success();
        }
    }
}
