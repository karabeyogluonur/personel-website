using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Constants;
using PW.Application.Interfaces.Caching;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Common;
using PW.Domain.Entities;
using PW.Domain.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace PW.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public LocalizationService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _localizedPropertyRepository = unitOfWork.GetRepository<LocalizedProperty>();
            _cacheService = cacheService;
        }

        public async Task<string> GetLocalizedAsync<T>(T entity, Expression<Func<T, string>> keySelector, int languageId)
            where T : BaseEntity, ILocalizedEntity
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            string localeKey = propInfo.Name;
            string localeKeyGroup = typeof(T).Name;

            string cacheKey = $"{CacheKeys.Localization.Property}:{localeKeyGroup}:{entity.Id}:{languageId}:{localeKey}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var prop = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: x =>
                    x.LanguageId == languageId &&
                    x.EntityId == entity.Id &&
                    x.LocaleKeyGroup == localeKeyGroup &&
                    x.LocaleKey == localeKey
                );

                if (prop != null && !string.IsNullOrEmpty(prop.LocaleValue))
                {
                    return prop.LocaleValue;
                }

                var defaultValue = (string)propInfo.GetValue(entity);
                return defaultValue ?? string.Empty;

            }, CacheDurations.Long);
        }

        public async Task SaveLocalizedValueAsync<T>(T entity, Expression<Func<T, string>> keySelector, string localeValue, int languageId)
            where T : BaseEntity, ILocalizedEntity
        {
            var member = keySelector.Body as MemberExpression;
            var propInfo = member.Member as PropertyInfo;
            string localeKey = propInfo.Name;
            string localeKeyGroup = typeof(T).Name;

            var prop = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: x =>
                    x.LanguageId == languageId &&
                    x.EntityId == entity.Id &&
                    x.LocaleKeyGroup == localeKeyGroup &&
                    x.LocaleKey == localeKey
                );

            if (prop != null)
            {
                if (string.IsNullOrEmpty(localeValue))
                    _localizedPropertyRepository.Delete(prop);
                else
                    prop.LocaleValue = localeValue;

                _localizedPropertyRepository.Update(prop);
            }
            else
            {
                if (!string.IsNullOrEmpty(localeValue))
                {
                    prop = new LocalizedProperty
                    {
                        EntityId = entity.Id,
                        LanguageId = languageId,
                        LocaleKey = localeKey,
                        LocaleKeyGroup = localeKeyGroup,
                        LocaleValue = localeValue
                    };
                    await _localizedPropertyRepository.InsertAsync(prop);
                }
            }

            await _unitOfWork.CommitAsync();

            string propCacheKey = $"{CacheKeys.Localization.Property}:{localeKeyGroup}:{entity.Id}:{languageId}:{localeKey}";
            await _cacheService.RemoveAsync(propCacheKey);

            string dictCacheKey = $"{CacheKeys.Localization.Dictionary}:{localeKeyGroup}:{languageId}";
            await _cacheService.RemoveAsync(dictCacheKey);
        }

        public async Task<Dictionary<string, string>> GetLocalizedDictionaryAsync(string keyGroup, int languageId)
        {
            string cacheKey = $"{CacheKeys.Localization.Dictionary}:{keyGroup}:{languageId}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var translations = await _localizedPropertyRepository.GetAll()
                    .Where(x => x.LocaleKeyGroup == keyGroup && x.LanguageId == languageId)
                    .Select(x => new { x.LocaleKey, x.LocaleValue })
                    .ToListAsync();

                return translations.ToDictionary(x => x.LocaleKey, x => x.LocaleValue);
            }, CacheDurations.Long);
        }

        public async Task<Dictionary<int, string>> GetSettingsTranslationsAsync(List<int> settingIds, int languageId)
        {

            if (settingIds == null || !settingIds.Any())
                return new Dictionary<int, string>();

            var translations = await _localizedPropertyRepository.GetAll()
                .Where(x =>
                    x.LocaleKeyGroup == "Setting" &&
                    x.LocaleKey == "Value" &&
                    x.LanguageId == languageId &&
                    settingIds.Contains(x.EntityId))
                .Select(x => new { x.EntityId, x.LocaleValue })
                .ToListAsync();

            return translations.ToDictionary(x => x.EntityId, x => x.LocaleValue);
        }

        public async Task<List<LocalizedProperty>> GetTranslationsForListAsync(List<int> entityIds, string localeKeyGroup, int languageId)
        {
            if (entityIds == null || !entityIds.Any())
                return new List<LocalizedProperty>();

            var query = _localizedPropertyRepository.GetAll()
                .Where(x =>
                    x.LocaleKeyGroup == localeKeyGroup &&
                    x.LanguageId == languageId &&
                    entityIds.Contains(x.EntityId));

            return await query.ToListAsync();
        }
    }
}
