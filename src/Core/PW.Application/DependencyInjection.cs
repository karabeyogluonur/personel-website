using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Common.Interfaces;
using PW.Application.Features.Assets;
using PW.Application.Features.Auth;
using PW.Application.Features.Categories;
using PW.Application.Features.Configuration;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Features.Tags;
using PW.Application.Features.Technologies;
using PW.Application.Features.Users;
using PW.Domain.Entities;
using PW.Domain.Interfaces;

namespace PW.Application;

public static class DependencyInjection
{
   public static void AddApplicationServices(this IHostApplicationBuilder builder)
   {

      #region Features

      builder.Services.AddScoped<ILanguageService, LanguageService>();
      builder.Services.AddScoped<IAssetService, AssetService>();
      builder.Services.AddScoped<ISettingService, SettingService>();
      builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
      builder.Services.AddScoped<ITechnologyService, TechnologyService>();
      builder.Services.AddScoped<ICategoryService, CategoryService>();
      builder.Services.AddScoped<ITagService, TagService>();
      builder.Services.AddScoped<IUserService, UserService>();
      builder.Services.AddScoped<IAuthService, AuthService>();

      #endregion

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
}
