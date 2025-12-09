using PW.Domain.Entities;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        IQueryable<Language> GetAllPublishedLanguages();
        Task<IList<Language>> GetAllPublishedLanguagesAsync();
        Task<Language> GetLanguageByCodeAsync(string code);

    }
}
