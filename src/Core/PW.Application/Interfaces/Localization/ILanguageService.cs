using PW.Domain.Entities;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        Task<IList<Language>> GetAllPublishedLanguagesAsync();
        Task<IList<Language>> GetAllLanguagesAsync();
        Task<Language> GetLanguageByCodeAsync(string code);
        Task<Language> GetLanguageByIdAsync(int id);
        Task<Language> GetDefaultLanguageAsync();
        Task InsertLanguageAsync(Language language);
        Task UpdateLanguageAsync(Language language);
        Task DeleteLanguageAsync(Language language);

    }
}
