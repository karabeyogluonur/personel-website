using PW.Application.Models;
using PW.Application.Models.Dtos.Localization;

namespace PW.Application.Interfaces.Localization;

public interface ILanguageService
{
   Task<IList<LanguageSummaryDto>> GetAllLanguagesAsync();
   Task<IList<LanguageLookupDto>> GetLanguagesLookupAsync();
   Task<LanguageDetailDto?> GetLanguageByCodeAsync(string languageCode);
   Task<LanguageDetailDto?> GetLanguageByIdAsync(int languageId);
   Task<LanguageDetailDto?> GetDefaultLanguageAsync();
   Task<OperationResult> CreateLanguageAsync(LanguageCreateDto languageCreateDto);
   Task<OperationResult> UpdateLanguageAsync(LanguageUpdateDto languageUpdateDto);
   Task<OperationResult> DeleteLanguageAsync(int languageId);
}
