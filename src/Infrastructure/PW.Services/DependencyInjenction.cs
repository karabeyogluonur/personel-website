using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Messages;
using PW.Application.Interfaces.Storage;
using PW.Domain.Interfaces;
using PW.Services.Configuration;
using PW.Services.Localization;
using PW.Services.Storage;

namespace PW.Services
{
    public static class DependencyInjection
    {
        public static void AddServiceServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILanguageService, LanguageService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<ISettingService, SettingService>();
            builder.Services.AddScoped<ILocalizationService, LocalizationService>();
            builder.Services.AddScoped<IStorageService, LocalStorageService>();
            builder.Services.AddScoped<IAssetService, AssetService>();

            #region  Setting Registration

            var assembly = typeof(ISettings).Assembly;

            var settingTypes = assembly.GetTypes()
                .Where(t => typeof(ISettings).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in settingTypes)
            {
                builder.Services.AddScoped(type, provider =>
                {
                    var settingService = provider.GetRequiredService<ISettingService>();
                    var workContext = provider.GetRequiredService<IWorkContext>();
                    var currentLanguage = workContext.GetCurrentLanguageAsync().GetAwaiter().GetResult();
                    int currentLanguageId = currentLanguage?.Id ?? 0;

                    var method = settingService.GetType().GetMethod(nameof(ISettingService.LoadSettings));
                    var genericMethod = method?.MakeGenericMethod(type);

                    return genericMethod?.Invoke(settingService, new object[] { currentLanguageId });
                });
            }

            #endregion



        }
    }
}
