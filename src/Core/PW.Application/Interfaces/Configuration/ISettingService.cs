

using PW.Domain.Interfaces;

namespace PW.Application.Interfaces.Configuration
{
    public interface ISettingService
    {
        T LoadSetting<T>(int languageId = 0) where T : ISettings, new();
        Task SaveSettingAsync<T>(T settings) where T : ISettings;
        T GetSettingByKey<T>(string key, T defaultValue = default, int languageId = 0);
        Task SetSettingAsync<T>(string key, T value, bool isPublic = true);
    }
}
