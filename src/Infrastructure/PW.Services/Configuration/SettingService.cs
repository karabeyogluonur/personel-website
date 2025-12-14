using System.Linq.Expressions;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Caching;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Entities;
using PW.Domain.Interfaces;

namespace PW.Services.Configuration
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Setting> _settingRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheService _cacheService;

        public SettingService(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = _unitOfWork.GetRepository<Setting>();
            _localizedPropertyRepository = _unitOfWork.GetRepository<LocalizedProperty>();
            _localizationService = localizationService;
            _cacheService = cacheService;
        }

        public T LoadSettings<T>(int languageId = 0) where T : ISettings, new()
        {
            string key = $"{CacheKeys.Settings.All}:{typeof(T).Name}:{languageId}";

            return _cacheService.GetOrSetAsync(key, async () =>
            {
                var settings = new T();
                string prefix = typeof(T).GetSettingsKeyPrefix();

                var allSettings = await _settingRepository.GetAllAsync(predicate: x => x.Name.StartsWith(prefix));

                Dictionary<int, string> translations = null;

                if (languageId > 0 && allSettings.Any())
                {
                    var settingIds = allSettings.Select(x => x.Id).ToList();
                    translations = await _localizationService.GetSettingsTranslationsAsync(settingIds, languageId);
                }

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (!prop.CanWrite || !prop.CanRead) continue;

                    string settingKey = typeof(T).BuildSettingKey(prop.Name);
                    var setting = allSettings.FirstOrDefault(x => x.Name.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase));

                    if (setting == null) continue;

                    string valueStr = setting.Value;

                    if (translations != null && translations.ContainsKey(setting.Id))
                        valueStr = translations[setting.Id];

                    var value = valueStr.ToType(prop.PropertyType);

                    if (value != null)
                    {
                        prop.SetValue(settings, value);
                    }
                }
                return settings;

            }, CacheDurations.Long).GetAwaiter().GetResult();
        }

        public async Task SaveSettingsAsync<T>(T settings) where T : ISettings
        {
            string prefix = typeof(T).GetSettingsKeyPrefix();

            var existingSettings = await _settingRepository.GetAllAsync(
                predicate: x => x.Name.StartsWith(prefix),
                disableTracking: false);

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead) continue;

                string key = typeof(T).BuildSettingKey(prop.Name);
                dynamic value = prop.GetValue(settings);

                string valueStr = ((object)value).ToInvariantString();

                var setting = existingSettings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (setting != null)
                {
                    if (setting.Value != valueStr)
                        setting.Value = valueStr;
                }
                else
                {
                    await _settingRepository.InsertAsync(new Setting { Name = key, Value = valueStr, IsPublic = true });
                }
            }

            await _unitOfWork.CommitAsync();

            string cacheKey = $"{CacheKeys.Settings.All}:{typeof(T).Name}:0";
            await _cacheService.RemoveAsync(cacheKey);
        }

        public TProp GetSettingValue<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId = 0)
            where TSettings : ISettings, new()
        {
            string key = keySelector.GetSettingKey();
            TProp defaultValue = default;

            var setting = _settingRepository.GetFirstOrDefault(predicate: x => x.Name == key);

            if (setting != null)
            {
                string valueStr = setting.Value;

                if (languageId > 0)
                {
                    var localizedValue = _localizationService.GetLocalizedAsync(setting, x => x.Value, languageId).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(localizedValue))
                        valueStr = localizedValue;
                }

                return valueStr.ToType<TProp>();
            }
            return defaultValue;
        }

        public async Task<string> GetLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId)
            where TSettings : ISettings, new()
        {
            string key = keySelector.GetSettingKey();

            var setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: x => x.Name == key);
            if (setting == null) return null;

            var prop = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: x =>
                x.LanguageId == languageId &&
                x.EntityId == setting.Id &&
                x.LocaleKeyGroup == "Setting" &&
                x.LocaleKey == "Value");

            return prop?.LocaleValue;
        }

        public async Task SaveLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, string value, int languageId)
            where TSettings : ISettings, new()
        {
            string key = keySelector.GetSettingKey();

            var setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: x => x.Name == key, disableTracking: false);

            if (setting == null) return;

            var prop = await _localizedPropertyRepository.GetFirstOrDefaultAsync(predicate: x =>
                x.LanguageId == languageId &&
                x.EntityId == setting.Id &&
                x.LocaleKeyGroup == "Setting" &&
                x.LocaleKey == "Value",
                disableTracking: false);

            string valueStr = value ?? string.Empty;

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                    _localizedPropertyRepository.Delete(prop);
                else
                    prop.LocaleValue = valueStr;
            }
            else if (!string.IsNullOrWhiteSpace(valueStr))
            {
                await _localizedPropertyRepository.InsertAsync(new LocalizedProperty
                {
                    EntityId = setting.Id,
                    LanguageId = languageId,
                    LocaleKeyGroup = "Setting",
                    LocaleKey = "Value",
                    LocaleValue = valueStr
                });
            }

            await _unitOfWork.CommitAsync();

            string cacheKey = $"{CacheKeys.Settings.All}:{typeof(TSettings).Name}:{languageId}";
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}
