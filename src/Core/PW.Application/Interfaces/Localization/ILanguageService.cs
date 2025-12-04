using PW.Domain.Entities.Localization;

namespace PW.Application.Interfaces.Localization
{
    public interface ILanguageService
    {
        public IQueryable<Language> GetAllPublishedLanguages();
    }
}
