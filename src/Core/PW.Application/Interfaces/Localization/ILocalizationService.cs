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

        Task<IList<LocalizedProperty>> GetLocalizedPropertiesAsync(IList<int> entityIds, string localeKeyGroup, int? languageId = null);

        Task<IDictionary<string, string>> GetLocalizedDictionaryAsync(string localeKeyGroup, int languageId);

        Task<IDictionary<int, string>> GetLocalizedValuesByEntityIdAsync(IList<int> entityIds, string localeKeyGroup, string localeKey, int languageId);
    }
}
