using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Caching;
using PW.Application.Interfaces.Configuration;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Messages;
using PW.Application.Interfaces.Storage;
using PW.Application.Models.Dtos.Localization;
using PW.Domain.Interfaces;
using PW.Services.Caching;
using PW.Services.Configuration;
using PW.Services.Content;
using PW.Services.Localization;
using PW.Services.Storage;

namespace PW.Services;

public static class DependencyInjection
{
   public static void AddServiceServices(this IHostApplicationBuilder builder)
   {
      builder.Services.AddScoped<ILanguageService, LanguageService>();
      builder.Services.AddScoped<INotificationService, NotificationService>();
      builder.Services.AddScoped<IStorageService, LocalStorageService>();
      builder.Services.AddScoped<IAssetService, AssetService>();
      builder.Services.AddScoped<IFileProcessorService, FileProcessorService>();
      builder.Services.AddSingleton<ILocalCacheService, MemoryCacheManager>();

      builder.Services.AddScoped<ISettingService, SettingService>();
      builder.Services.AddScoped<IConfigurationService, ConfigurationService>();

      builder.Services.AddScoped<ITechnologyService, TechnologyService>();
      builder.Services.AddScoped<ICategoryService, CategoryService>();

      #region Setting Registration (Automated)

      Assembly assembly = typeof(ISettings).Assembly;

      IEnumerable<Type> settingTypes = assembly.GetTypes()
          .Where(type => typeof(ISettings).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

      foreach (Type type in settingTypes)
      {
         builder.Services.AddScoped(type, (IServiceProvider provider) =>
         {
            ISettingService settingService = provider.GetRequiredService<ISettingService>();
            IWorkContext workContext = provider.GetRequiredService<IWorkContext>();

            LanguageDetailDto? currentLanguage = workContext.GetCurrentLanguageAsync().GetAwaiter().GetResult();
            int currentLanguageId = currentLanguage?.Id ?? 0;

            MethodInfo? method = settingService.GetType().GetMethod(nameof(ISettingService.LoadSettings));
            MethodInfo? genericMethod = method?.MakeGenericMethod(type);

            return genericMethod?.Invoke(settingService, new object[] { currentLanguageId })!;
         });
      }

      #endregion
   }

   public static void AddMemoryCacheService(this IHostApplicationBuilder builder)
   {
      builder.Services.AddSingleton<ICacheService>((IServiceProvider provider) =>
              provider.GetRequiredService<ILocalCacheService>());
   }
}
