using Microsoft.Extensions.Logging;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Application.Models;
using PW.Domain.Entities;

namespace PW.Services.Localization
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Language> _languageRepository;
        private readonly ILogger<LanguageService> _logger;

        public LanguageService(IUnitOfWork unitOfWork, ILogger<LanguageService> logger)
        {
            _unitOfWork = unitOfWork;
            _languageRepository = _unitOfWork.GetRepository<Language>();
            _logger = logger;
        }

        public async Task<IList<Language>> GetAllLanguagesAsync()
        {
            return await _languageRepository.GetAllAsync(
                orderBy: query => query.OrderBy(language => language.DisplayOrder)
            );
        }

        public async Task<IList<Language>> GetAllPublishedLanguagesAsync()
        {
            return await _languageRepository.GetAllAsync(
                predicate: language => language.IsPublished,
                orderBy: query => query.OrderBy(language => language.DisplayOrder)
            );
        }

        public async Task<Language> GetLanguageByCodeAsync(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return null;

            return await _languageRepository.GetFirstOrDefaultAsync(
                predicate: language => language.Code == languageCode
            );
        }

        public async Task<Language> GetLanguageByIdAsync(int languageId)
        {
            return await _languageRepository.GetFirstOrDefaultAsync(
                predicate: language => language.Id == languageId
            );
        }

        public async Task<Language> GetDefaultLanguageAsync()
        {
            return await _languageRepository.GetFirstOrDefaultAsync(
                predicate: language => language.IsDefault
            );
        }

        public async Task<OperationResult> InsertLanguageAsync(Language language)
        {
            if (language is null)
                throw new ArgumentNullException(nameof(language));

            bool isCodeExists = await _languageRepository.ExistsAsync(
                 existingLanguage => existingLanguage.Code == language.Code
            );

            if (isCodeExists)
                return OperationResult.Failure($"Language code '{language.Code}' already exists.", OperationErrorType.Conflict);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (language.IsDefault)
                    await UnsetDefaultLanguagesAsync();

                await _languageRepository.InsertAsync(language);
                await _unitOfWork.CommitTransactionAsync();

                return OperationResult.Success();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error inserting language: {LanguageCode}", language.Code);
                await _unitOfWork.RollbackTransactionAsync();
                return OperationResult.Failure("The language could not be added due to a system error.", OperationErrorType.Technical);
            }
        }

        public async Task<OperationResult> UpdateLanguageAsync(Language language)
        {
            if (language is null)
                throw new ArgumentNullException(nameof(language));

            var originalLanguage = await _languageRepository.GetFirstOrDefaultAsync(
                predicate: l => l.Id == language.Id,
                disableTracking: false
            );

            if (originalLanguage is null)
                return OperationResult.Failure("The language to be updated was not found.", OperationErrorType.NotFound);

            if (!string.Equals(originalLanguage.Code, language.Code, StringComparison.OrdinalIgnoreCase))
            {
                bool isDuplicate = await _languageRepository.ExistsAsync(
                     l => l.Code == language.Code
                );

                if (isDuplicate)
                    return OperationResult.Failure($"Language code '{language.Code}' already exists.", OperationErrorType.Conflict);
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (language.IsDefault)
                    await UnsetDefaultLanguagesAsync(excludeLanguageId: language.Id);

                _languageRepository.Update(language);

                await _unitOfWork.CommitTransactionAsync();

                return OperationResult.Success();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating language: {LanguageCode}", language.Code);
                await _unitOfWork.RollbackTransactionAsync();
                return OperationResult.Failure("The update could not be performed due to a system error.", OperationErrorType.Technical);
            }
        }

        public async Task<OperationResult> DeleteLanguageAsync(Language language)
        {
            if (language is null)
                throw new ArgumentNullException(nameof(language));

            if (language.IsDefault)
                return OperationResult.Failure("The default language cannot be deleted. Please set another language as default first.", OperationErrorType.BusinessRule);

            try
            {
                _languageRepository.Delete(language);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error deleting language: {LanguageId}", language.Id);
                return OperationResult.Failure("A technical error occurred while deleting the language.", OperationErrorType.Technical);
            }
        }

        private async Task UnsetDefaultLanguagesAsync(int? excludeLanguageId = null)
        {
            var defaultLanguages = await _languageRepository.GetAllAsync(
                predicate: language => language.IsDefault,
                disableTracking: false
            );

            bool anyLanguageChanged = false;

            foreach (var language in defaultLanguages)
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
}
