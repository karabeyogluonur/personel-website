using PW.Application.Models;
using PW.Domain.Entities;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        Task<IList<Language>> GetAllPublishedLanguagesAsync();
        Task<IList<Language>> GetAllLanguagesAsync();
        Task<Language> GetLanguageByCodeAsync(string languageCode);
        Task<Language> GetLanguageByIdAsync(int languageId);
        Task<Language> GetDefaultLanguageAsync();
        Task<OperationResult> InsertLanguageAsync(Language language);
        Task<OperationResult> UpdateLanguageAsync(Language language);
        Task<OperationResult> DeleteLanguageAsync(Language language);

    }
}
