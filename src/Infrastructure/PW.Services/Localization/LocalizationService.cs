using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Constants;
using PW.Application.Interfaces.Caching;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Common;
using PW.Domain.Entities;
using PW.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PW.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly ICacheService _cacheService;

        public LocalizationService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _localizedPropertyRepository = _unitOfWork.GetRepository<LocalizedProperty>();
            _cacheService = cacheService;
        }

        public async Task<string> GetLocalizedAsync<T>(T entity, Expression<Func<T, string>> keySelector, int languageId)
            where T : BaseEntity, ILocalizedEntity
        {
            MemberExpression? memberExpression = keySelector.Body as MemberExpression;
            if (memberExpression is null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            PropertyInfo? propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo is null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            string localeKey = propertyInfo.Name;
            string localeKeyGroup = typeof(T).Name;
            string cacheKey = $"{CacheKeys.Localization.Property}:{localeKeyGroup}:{entity.Id}:{languageId}:{localeKey}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                LocalizedProperty? localizedProperty = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: property =>
                    property.LanguageId == languageId &&
                    property.EntityId == entity.Id &&
                    property.LocaleKeyGroup == localeKeyGroup &&
                    property.LocaleKey == localeKey
                );

                if (localizedProperty is not null && !string.IsNullOrEmpty(localizedProperty.LocaleValue))
                    return localizedProperty.LocaleValue;

                string? defaultValue = (string?)propertyInfo.GetValue(entity);
                return defaultValue ?? string.Empty;

            }, CacheDurations.Long);
        }

        public async Task SaveLocalizedValueAsync<T>(T entity, Expression<Func<T, string>> keySelector, string localeValue, int languageId)
            where T : BaseEntity, ILocalizedEntity
        {
            MemberExpression? memberExpression = keySelector.Body as MemberExpression;
            PropertyInfo? propertyInfo = memberExpression?.Member as PropertyInfo;

            if (propertyInfo is null) return;

            string localeKey = propertyInfo.Name;
            string localeKeyGroup = typeof(T).Name;

            LocalizedProperty localizedProperty = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: property =>
                property.LanguageId == languageId &&
                property.EntityId == entity.Id &&
                property.LocaleKeyGroup == localeKeyGroup &&
                property.LocaleKey == localeKey
            );

            if (localizedProperty is not null)
            {
                if (string.IsNullOrEmpty(localeValue))
                    _localizedPropertyRepository.Delete(localizedProperty);
                else
                {
                    localizedProperty.LocaleValue = localeValue;
                    _localizedPropertyRepository.Update(localizedProperty);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(localeValue))
                {
                    LocalizedProperty newProperty = new LocalizedProperty
                    {
                        EntityId = entity.Id,
                        LanguageId = languageId,
                        LocaleKey = localeKey,
                        LocaleKeyGroup = localeKeyGroup,
                        LocaleValue = localeValue
                    };
                    await _localizedPropertyRepository.InsertAsync(newProperty);
                }
            }

            await _unitOfWork.CommitAsync();

            // Cache Invalidation
            string propertyCacheKey = $"{CacheKeys.Localization.Property}:{localeKeyGroup}:{entity.Id}:{languageId}:{localeKey}";
            await _cacheService.RemoveAsync(propertyCacheKey);

            string dictionaryCacheKey = $"{CacheKeys.Localization.Dictionary}:{localeKeyGroup}:{languageId}";
            await _cacheService.RemoveAsync(dictionaryCacheKey);
        }

        public async Task<IList<LocalizedProperty>> GetLocalizedPropertiesAsync(IList<int> entityIds, string localeKeyGroup, int? languageId = null)
        {
            if (entityIds is null || !entityIds.Any())
                return new List<LocalizedProperty>();

            IQueryable<LocalizedProperty> query = _localizedPropertyRepository.GetAll();

            query = query.Where(property =>
                property.LocaleKeyGroup == localeKeyGroup &&
                entityIds.Contains(property.EntityId));

            if (languageId.HasValue && languageId.Value > 0)
            {
                query = query.Where(property => property.LanguageId == languageId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IDictionary<string, string>> GetLocalizedDictionaryAsync(string localeKeyGroup, int languageId)
        {
            string cacheKey = $"{CacheKeys.Localization.Dictionary}:{localeKeyGroup}:{languageId}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                // Explicit Type kullanımı: List<AnonymousType> yerine açıkça veri çekip dictionary'e çeviriyoruz.
                IList<LocalizedProperty> properties = await _localizedPropertyRepository.GetAll()
                    .Where(property => property.LocaleKeyGroup == localeKeyGroup && property.LanguageId == languageId)
                    .ToListAsync();

                return properties.ToDictionary(k => k.LocaleKey, v => v.LocaleValue ?? string.Empty);

            }, CacheDurations.Long);
        }

        public async Task<IDictionary<int, string>> GetLocalizedValuesByEntityIdAsync(IList<int> entityIds, string localeKeyGroup, string localeKey, int languageId)
        {
            if (entityIds is null || !entityIds.Any())
                return new Dictionary<int, string>();

            IList<LocalizedProperty> properties = await _localizedPropertyRepository.GetAll()
                .Where(property =>
                    property.LocaleKeyGroup == localeKeyGroup &&
                    property.LocaleKey == localeKey &&
                    property.LanguageId == languageId &&
                    entityIds.Contains(property.EntityId))
                .ToListAsync();

            return properties.ToDictionary(k => k.EntityId, v => v.LocaleValue ?? string.Empty);
        }
    }
}
