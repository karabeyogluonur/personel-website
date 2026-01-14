using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Application.Utilities.Results;
using PW.Domain.Entities;

namespace PW.Application.Features.Localization;

public class LanguageService : ILanguageService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IRepository<Language> _languageRepository;
   private readonly IFileProcessorService _fileProcessorService;

   public LanguageService(IUnitOfWork unitOfWork, IFileProcessorService fileProcessorService)
   {
      _unitOfWork = unitOfWork;
      _languageRepository = _unitOfWork.GetRepository<Language>();
      _fileProcessorService = fileProcessorService;
   }

   public async Task<IList<LanguageSummaryDto>> GetAllLanguagesAsync()
   {
      IList<Language> languages = await _languageRepository.GetAllAsync(
          orderBy: query => query.OrderBy(language => language.DisplayOrder)
      );

      IList<LanguageSummaryDto> result = languages.Select(language => new LanguageSummaryDto
      {
         Id = language.Id,
         Name = language.Name,
         Code = language.Code,
         FlagImageFileName = language.FlagImageFileName,
         IsPublished = language.IsPublished,
         IsDefault = language.IsDefault,
         DisplayOrder = language.DisplayOrder,
         CreatedAt = language.CreatedAt,
         UpdatedAt = language.UpdatedAt
      }).ToList();

      return result;
   }

   public async Task<IList<LanguageLookupDto>> GetLanguagesLookupAsync()
   {
      IList<Language> languages = await _languageRepository.GetAllAsync(
          predicate: (Language language) => language.IsPublished,
          orderBy: (IQueryable<Language> query) => query.OrderBy((Language language) => language.DisplayOrder)
      );

      return languages.Select((Language language) => new LanguageLookupDto
      {
         Id = language.Id,
         Name = language.Name,
         Code = language.Code,
         FlagImageFileName = language.FlagImageFileName,
         IsDefault = language.IsDefault
      }).ToList();
   }

   public async Task<LanguageDetailDto?> GetLanguageByCodeAsync(string languageCode)
   {
      if (string.IsNullOrWhiteSpace(languageCode))
         return null;

      Language? language = await _languageRepository.GetFirstOrDefaultAsync(
          predicate: (Language language) => language.Code == languageCode
      );

      if (language == null)
         return null;

      return new LanguageDetailDto
      {
         Id = language.Id,
         Name = language.Name,
         Code = language.Code,
         FlagImageFileName = language.FlagImageFileName,
         IsPublished = language.IsPublished,
         IsDefault = language.IsDefault,
         DisplayOrder = language.DisplayOrder,
         CreatedAt = language.CreatedAt,
         UpdatedAt = language.UpdatedAt
      };
   }

   public async Task<LanguageDetailDto?> GetLanguageByIdAsync(int languageId)
   {
      Language language = await _languageRepository.GetFirstOrDefaultAsync(predicate: language => language.Id == languageId);

      if (language == null) return null;

      return new LanguageDetailDto
      {
         Id = language.Id,
         Name = language.Name,
         Code = language.Code,
         IsPublished = language.IsPublished,
         IsDefault = language.IsDefault,
         DisplayOrder = language.DisplayOrder,
         FlagImageFileName = language.FlagImageFileName,
         CreatedAt = language.CreatedAt,
         UpdatedAt = language.UpdatedAt
      };
   }

   public async Task<LanguageDetailDto?> GetDefaultLanguageAsync()
   {
      Language? language = await _languageRepository.GetFirstOrDefaultAsync(
          predicate: (Language language) => language.IsDefault
      );

      if (language == null)
         return null;

      return new LanguageDetailDto
      {
         Id = language.Id,
         Name = language.Name,
         Code = language.Code,
         FlagImageFileName = language.FlagImageFileName,
         IsPublished = language.IsPublished,
         IsDefault = language.IsDefault,
         DisplayOrder = language.DisplayOrder,
         CreatedAt = language.CreatedAt,
         UpdatedAt = language.UpdatedAt
      };
   }

   public async Task<OperationResult> CreateLanguageAsync(LanguageCreateDto languageCreateDto)
   {
      if (languageCreateDto == null)
         throw new ArgumentNullException(nameof(languageCreateDto));

      bool isCodeExists = await _languageRepository.ExistsAsync(
            existingLanguage => existingLanguage.Code == languageCreateDto.Code
      );

      if (isCodeExists)
         return OperationResult.Failure($"Language code '{languageCreateDto.Code}' already exists.", OperationErrorType.Conflict);

      string uploadedFlagFileName = string.Empty;

      await _unitOfWork.BeginTransactionAsync();

      try
      {
         if (languageCreateDto.IsDefault)
            await UnsetDefaultLanguagesAsync(excludeLanguageId: null);

         uploadedFlagFileName = await _fileProcessorService.HandleFileUpdateAsync(
             fileInput: languageCreateDto.FlagImage,
             currentDbFileName: null,
             mode: FileNamingMode.Specific,
             folderPath: StoragePaths.System_Flags,
             slugName: languageCreateDto.Code
         );

         Language language = new Language
         {
            Name = languageCreateDto.Name,
            Code = languageCreateDto.Code,
            IsPublished = languageCreateDto.IsPublished,
            IsDefault = languageCreateDto.IsDefault,
            DisplayOrder = languageCreateDto.DisplayOrder,
            FlagImageFileName = uploadedFlagFileName,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
         };

         await _languageRepository.InsertAsync(language);
         await _unitOfWork.CommitTransactionAsync();

         return OperationResult.Success();
      }
      catch (Exception)
      {
         await _unitOfWork.RollbackTransactionAsync();

         if (!string.IsNullOrEmpty(uploadedFlagFileName))
            await _fileProcessorService.DeleteFileAsync(StoragePaths.System_Flags, uploadedFlagFileName);

         return OperationResult.Failure("The language could not be added due to a system error.", OperationErrorType.Technical);
      }
   }

   public async Task<OperationResult> UpdateLanguageAsync(LanguageUpdateDto languageUpdateDto)
   {
      if (languageUpdateDto == null)
         throw new ArgumentNullException(nameof(languageUpdateDto));

      Language originalLanguage = await _languageRepository.GetFirstOrDefaultAsync(
          predicate: language => language.Id == languageUpdateDto.LanguageId,
          disableTracking: false
      );

      if (originalLanguage == null)
         return OperationResult.Failure("The language to be updated was not found.", OperationErrorType.NotFound);

      if (!string.Equals(originalLanguage.Code, languageUpdateDto.Code, StringComparison.OrdinalIgnoreCase))
      {
         bool isDuplicate = await _languageRepository.ExistsAsync(language => language.Code == languageUpdateDto.Code);

         if (isDuplicate)
            return OperationResult.Failure($"Language code '{languageUpdateDto.Code}' already exists.", OperationErrorType.Conflict);
      }

      await _unitOfWork.BeginTransactionAsync();

      if (languageUpdateDto.IsDefault && !originalLanguage.IsDefault)
         await UnsetDefaultLanguagesAsync(excludeLanguageId: originalLanguage.Id);

      originalLanguage.FlagImageFileName = await _fileProcessorService.HandleFileUpdateAsync(
          fileInput: languageUpdateDto.FlagImage,
          currentDbFileName: originalLanguage.FlagImageFileName,
          mode: FileNamingMode.Specific,
          folderPath: StoragePaths.System_Flags,
          slugName: languageUpdateDto.Code
      );

      originalLanguage.Name = languageUpdateDto.Name;
      originalLanguage.Code = languageUpdateDto.Code;
      originalLanguage.IsPublished = languageUpdateDto.IsPublished;
      originalLanguage.IsDefault = languageUpdateDto.IsDefault;
      originalLanguage.DisplayOrder = languageUpdateDto.DisplayOrder;
      originalLanguage.UpdatedAt = DateTime.UtcNow;

      _languageRepository.Update(originalLanguage);
      await _unitOfWork.CommitTransactionAsync();

      return OperationResult.Success();
   }

   public async Task<OperationResult> DeleteLanguageAsync(int languageId)
   {
      Language language = await _languageRepository.GetFirstOrDefaultAsync(predicate: language => language.Id == languageId);

      if (language == null)
         return OperationResult.Failure("Language not found.", OperationErrorType.NotFound);

      if (language.IsDefault)
         return OperationResult.Failure("The default language cannot be deleted. Please set another language as default first.", OperationErrorType.BusinessRule);

      if (!string.IsNullOrEmpty(language.FlagImageFileName))
         await _fileProcessorService.DeleteFileAsync(StoragePaths.System_Flags, language.FlagImageFileName);

      _languageRepository.Delete(language);
      await _unitOfWork.CommitAsync();

      return OperationResult.Success();
   }

   private async Task UnsetDefaultLanguagesAsync(int? excludeLanguageId)
   {
      IList<Language> defaultLanguages = await _languageRepository.GetAllAsync(
          predicate: language => language.IsDefault,
          disableTracking: false
      );

      bool anyLanguageChanged = false;

      foreach (Language language in defaultLanguages)
      {
         if (excludeLanguageId.HasValue && language.Id == excludeLanguageId.Value)
            continue;

         language.IsDefault = false;
         anyLanguageChanged = true;
      }

      if (anyLanguageChanged)
         _languageRepository.Update(defaultLanguages);
   }
}
