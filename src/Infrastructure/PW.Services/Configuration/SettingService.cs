using System.ComponentModel;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Entities;
using PW.Domain.Interfaces;

namespace PW.Services.Configuration
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Setting> _settingRepository;

        public SettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = _unitOfWork.GetRepository<Setting>();
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            var settings = new T();

            string prefix = GetPrefix<T>();

            var allSettings = _settingRepository.GetAll()
                .Where(x => x.Name.StartsWith(prefix))
                .ToList();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                string key = $"{prefix}.{prop.Name}";

                var setting = allSettings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (setting == null)
                    continue;

                var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);

                if (typeConverter.CanConvertFrom(typeof(string)))
                {
                    object value = typeConverter.ConvertFrom(setting.Value);
                    prop.SetValue(settings, value);
                }
            }

            return settings;
        }

        public async Task SaveSettingAsync<T>(T settings) where T : ISettings
        {
            string prefix = GetPrefix<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead)
                    continue;

                string key = $"{prefix}.{prop.Name}";

                dynamic value = prop.GetValue(settings);

                string valueStr = value != null ? value.ToString() : "";

                await SetSettingAsync(key, valueStr);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task SetSettingAsync<T>(string key, T value, bool isPublic = true)
        {
            string valueStr = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value);

            var setting = _settingRepository.GetFirstOrDefault(predicate: x => x.Name == key);

            if (setting != null)
            {
                setting.Value = valueStr;
                _settingRepository.Update(setting);
            }
            else
            {
                var newSetting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    IsPublic = isPublic
                };
                await _settingRepository.InsertAsync(newSetting);
                await _unitOfWork.CommitAsync();
            }
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default)
        {
            var setting = _settingRepository.GetFirstOrDefault(predicate: x => x.Name == key);

            if (setting != null)
            {
                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                if (typeConverter.CanConvertFrom(typeof(string)))
                {
                    return (T)typeConverter.ConvertFrom(setting.Value);
                }
            }
            return defaultValue;
        }

        private string GetPrefix<T>()
        {
            string className = typeof(T).Name;

            if (className.EndsWith("Settings"))
                className = className.Substring(0, className.Length - "Settings".Length);

            return className;
        }

    }
}
