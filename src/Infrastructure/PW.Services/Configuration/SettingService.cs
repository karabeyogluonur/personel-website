using System.ComponentModel;
using System.Linq.Expressions;
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

        public SettingService(IUnitOfWork unitOfWork, ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = _unitOfWork.GetRepository<Setting>();
            _localizedPropertyRepository = _unitOfWork.GetRepository<LocalizedProperty>();
            _localizationService = localizationService;
        }

        #region Utilities
        private string GetPrefix<T>()
        {
            string className = typeof(T).Name;
            if (className.EndsWith("Settings"))
                className = className.Substring(0, className.Length - "Settings".Length);
            return className;
        }

        private string GetKey<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector)
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            string className = typeof(TSettings).Name;
            if (className.EndsWith("Settings"))
                className = className.Substring(0, className.Length - "Settings".Length);

            return $"{className}.{member.Member.Name}";
        }
        #endregion

        public T LoadSettings<T>(int languageId = 0) where T : ISettings, new()
        {
            var settings = new T();
            string prefix = GetPrefix<T>();

            var allSettings = _settingRepository.GetAll(predicate: x => x.Name.StartsWith(prefix)).ToList();

            Dictionary<int, string> translations = null;

            if (languageId > 0 && allSettings.Any())
            {
                var settingIds = allSettings.Select(x => x.Id).ToList();
                translations = _localizationService.GetSettingsTranslationsAsync(settingIds, languageId).GetAwaiter().GetResult();
            }

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanWrite || !prop.CanRead) continue;

                string key = $"{prefix}.{prop.Name}";
                var setting = allSettings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (setting == null) continue;

                string valueStr = setting.Value;

                if (translations != null && translations.ContainsKey(setting.Id))
                    valueStr = translations[setting.Id];

                var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                if (typeConverter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        object value = typeConverter.ConvertFrom(valueStr);
                        prop.SetValue(settings, value);
                    }
                    catch { }
                }
            }

            return settings;
        }

        public async Task SaveSettingsAsync<T>(T settings) where T : ISettings
        {
            string prefix = GetPrefix<T>();

            var existingSettings = await _settingRepository.GetAllAsync(
                predicate: x => x.Name.StartsWith(prefix),
                disableTracking: false);

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead) continue;

                string key = $"{prefix}.{prop.Name}";
                dynamic value = prop.GetValue(settings);
                string valueStr = TypeDescriptor.GetConverter(prop.PropertyType).ConvertToInvariantString(value);

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
        }

        public TProp GetSettingValue<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId = 0)
            where TSettings : ISettings, new()
        {
            string key = GetKey(keySelector);
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

                var typeConverter = TypeDescriptor.GetConverter(typeof(TProp));
                if (typeConverter.CanConvertFrom(typeof(string)))
                {
                    try { return (TProp)typeConverter.ConvertFrom(valueStr); }
                    catch { return defaultValue; }
                }
            }
            return defaultValue;
        }

        public async Task<string> GetLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId)
            where TSettings : ISettings, new()
        {
            string key = GetKey(keySelector);

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
            string key = GetKey(keySelector);

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
        }
    }
}
