using System.Linq.Expressions;
using System.Reflection;
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
        private readonly ILocalizationService _localizationService;
        private readonly ICacheService _cacheService;

        public SettingService(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = _unitOfWork.GetRepository<Setting>();
            _localizationService = localizationService;
            _cacheService = cacheService;
        }

        public T LoadSettings<T>(int languageId = 0) where T : ISettings, new()
        {
            string cacheKey = $"{CacheKeys.Settings.All}:{typeof(T).Name}:{languageId}";

            return _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                T settings = new T();
                string prefix = typeof(T).GetSettingsKeyPrefix();

                IList<Setting> allSettings = await _settingRepository.GetAllAsync(predicate: setting => setting.Name.StartsWith(prefix));

                IDictionary<int, string>? translations = null;

                if (languageId > 0 && allSettings.Any())
                {
                    IList<int> settingIds = allSettings.Select(s => s.Id).ToList();

                    translations = await _localizationService.GetLocalizedValuesByEntityIdAsync(
                        settingIds,
                        localeKeyGroup: nameof(Setting),
                        localeKey: nameof(Setting.Value),
                        languageId: languageId
                    );
                }

                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    if (!propertyInfo.CanWrite || !propertyInfo.CanRead) continue;

                    string settingKey = typeof(T).BuildSettingKey(propertyInfo.Name);

                    Setting? setting = allSettings.FirstOrDefault(x => x.Name.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase));

                    if (setting is null) continue;

                    string valueStr = setting.Value;

                    if (translations is not null && translations.TryGetValue(setting.Id, out var localizedValue) && !string.IsNullOrWhiteSpace(localizedValue))
                        valueStr = localizedValue;

                    object? typedValue = valueStr.ToType(propertyInfo.PropertyType);

                    if (typedValue is not null)
                        propertyInfo.SetValue(settings, typedValue);
                }

                return settings;

            }, CacheDurations.Long).GetAwaiter().GetResult();
        }

        public async Task SaveSettingsAsync<T>(T settings) where T : ISettings
        {
            string prefix = typeof(T).GetSettingsKeyPrefix();

            IList<Setting> existingSettings = await _settingRepository.GetAllAsync(
                predicate: x => x.Name.StartsWith(prefix),
                disableTracking: false);

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (!propertyInfo.CanRead) continue;

                string key = typeof(T).BuildSettingKey(propertyInfo.Name);
                object? value = propertyInfo.GetValue(settings);

                string valueStr = value?.ToInvariantString() ?? string.Empty;

                Setting? setting = existingSettings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (setting is not null)
                {
                    if (setting.Value != valueStr)
                        setting.Value = valueStr;
                }
                else
                {
                    Setting newSetting = new Setting
                    {
                        Name = key,
                        Value = valueStr,
                        IsPublic = true
                    };
                    await _settingRepository.InsertAsync(newSetting);
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
            TProp? defaultValue = default;

            Setting? setting = _settingRepository.GetFirstOrDefault(predicate: x => x.Name == key);

            if (setting is not null)
            {
                string valueStr = setting.Value;

                if (languageId > 0)
                {
                    string localizedValue = _localizationService.GetLocalizedAsync(setting, x => x.Value, languageId).GetAwaiter().GetResult();

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

            Setting? setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: x => x.Name == key);

            if (setting is null) return string.Empty;

            return await _localizationService.GetLocalizedAsync(setting, x => x.Value, languageId);
        }

        public async Task SaveLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, string value, int languageId)
            where TSettings : ISettings, new()
        {
            string key = keySelector.GetSettingKey();

            Setting? setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: x => x.Name == key);

            if (setting is null) return;

            await _localizationService.SaveLocalizedValueAsync(setting, x => x.Value, value, languageId);

            string cacheKey = $"{CacheKeys.Settings.All}:{typeof(TSettings).Name}:{languageId}";
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}
