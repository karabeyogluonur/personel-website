using System.Linq.Expressions;
using PW.Domain.Interfaces;

namespace PW.Application.Features.Configuration;

public interface ISettingService
{
   T LoadSettings<T>(int languageId = 0) where T : ISettings, new();

   Task SaveSettingsAsync<T>(T settings) where T : ISettings;

   TProp GetSettingValue<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId = 0)
       where TSettings : ISettings, new();

   Task<string> GetLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, int languageId)
       where TSettings : ISettings, new();

   Task SaveLocalizedSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, string value, int languageId)
       where TSettings : ISettings, new();
}
