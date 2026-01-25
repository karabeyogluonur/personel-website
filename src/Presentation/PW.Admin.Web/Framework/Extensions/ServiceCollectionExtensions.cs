using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using PW.Application.Common.Interfaces;
using PW.Admin.Web.Features.Auth.Services;
using PW.Admin.Web.Features.Categories.Services;
using PW.Admin.Web.Features.Configuration.Services;
using PW.Admin.Web.Features.Languages.Services;
using PW.Admin.Web.Features.Tags.Services;
using PW.Admin.Web.Features.Technologies.Services;
using PW.Admin.Web.Features.Users.Services;
using PW.Admin.Web.Services.Contexts;
using PW.Admin.Web.Services.Messages;
using PW.Web.Framework.Extensions;

namespace PW.Admin.Web.Framework.Extensions;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddAdminProjectServices(this IServiceCollection services, WebApplicationBuilder builder)
   {
      services.AddSharedInfrastructure(builder);
      services.AddSharedWebComponents(Assembly.GetExecutingAssembly());
      services.AddAdminSpecifics();
      services.AddAdminOrchestrators();
      services.ConfigureAdminCookie();

      return services;
   }

   private static IServiceCollection AddAdminSpecifics(this IServiceCollection services)
   {
      services.AddScoped<IWorkContext, WorkContext>();
      services.AddScoped<INotificationService, NotificationService>();
      services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

      return services;
   }

   private static IServiceCollection AddAdminOrchestrators(this IServiceCollection services)
   {
      services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
      services.AddScoped<IUserOrchestrator, UserOrchestrator>();
      services.AddScoped<ILanguageOrchestrator, LanguageOrchestrator>();
      services.AddScoped<IProfileSettingsOrchestrator, ProfileSettingsOrchestrator>();
      services.AddScoped<IGeneralSettingsOrchestrator, GeneralSettingsOrchestrator>();
      services.AddScoped<ITechnologyOrchestrator, TechnologyOrchestrator>();
      services.AddScoped<ICategoryOrchestrator, CategoryOrchestrator>();
      services.AddScoped<ITagOrchestrator, TagOrchestrator>();
      return services;
   }

   private static IServiceCollection ConfigureAdminCookie(this IServiceCollection services)
   {
      services.ConfigureApplicationCookie(options =>
      {
         options.Cookie.Name = "PW.Admin.Auth";
         options.AccessDeniedPath = "/Error/Forbidden";
         options.LoginPath = "/Auth/Login";
         options.LogoutPath = "/Auth/Logout";
         options.Cookie.HttpOnly = true;
         options.Cookie.SameSite = SameSiteMode.Strict;
         options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
         options.SlidingExpiration = true;
      });
      return services;
   }
}
