using PW.Domain.Entities;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        public IQueryable<Language> GetAllPublishedLanguages();
    }
}
