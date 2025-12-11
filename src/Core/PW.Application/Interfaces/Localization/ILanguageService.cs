using PW.Domain.Entities;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        IQueryable<Language> GetAllPublishedLanguages();
        Task<IList<Language>> GetAllPublishedLanguagesAsync();
        Task<IList<Language>> GetAllLanguagesAsync();
        Task<Language> GetLanguageByCodeAsync(string code);
        Task<Language> GetLanguageByIdAsync(int id);
        Task InsertLanguageAsync(Language language);
        Task UpdateLanguageAsync(Language language);
        Task DeleteLanguageAsync(Language language);

    }
}
