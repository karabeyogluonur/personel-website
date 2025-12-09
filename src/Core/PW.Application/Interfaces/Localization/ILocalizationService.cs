using System.Linq.Expressions;
using PW.Domain.Common;
using PW.Domain.Entities;
using PW.Domain.Interfaces;

namespace PW.Application.Interfaces.Localization
{
    public interface ILocalizationService
    {
        Task<string> GetLocalizedAsync<T>(T entity, Expression<Func<T, string>> keySelector, int languageId)
            where T : BaseEntity, ILocalizedEntity;

        Task SaveLocalizedValueAsync<T>(T entity, Expression<Func<T, string>> keySelector, string localeValue, int languageId)
            where T : BaseEntity, ILocalizedEntity;

        Task<Dictionary<string, string>> GetLocalizedDictionaryAsync(string keyGroup, int languageId);

        Task<Dictionary<int, string>> GetSettingsTranslationsAsync(List<int> settingIds, int languageId);

        Task<List<LocalizedProperty>> GetTranslationsForListAsync(List<int> entityIds, string localeKeyGroup, int languageId);
    }
}
