using PW.Domain.Entities;

namespace PW.Application.Common.Interfaces
{
    public interface IWorkContext
    {
        Task<Language> GetCurrentLanguageAsync();
        Task SetCurrentLanguageAsync(Language language);
    }
}
