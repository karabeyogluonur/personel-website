using System.Reflection;
using PW.Application.Common.Interfaces;
using PW.Public.Web.Services.Contexts;
using PW.Web.Framework.Extensions;

namespace PW.Public.Web.Framework.Extensions;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddPublicProjectServices(this IServiceCollection services, WebApplicationBuilder builder)
   {
      services.AddSharedInfrastructure(builder);
      services.AddSharedWebComponents(Assembly.GetExecutingAssembly());
      services.AddPublicSpecifics();
      services.ConfigurePublicCookie();

      return services;
   }

   private static IServiceCollection AddPublicSpecifics(this IServiceCollection services)
   {
      services.AddScoped<IWorkContext, WorkContext>();
      return services;
   }

   private static IServiceCollection ConfigurePublicCookie(this IServiceCollection services)
   {
      services.ConfigureApplicationCookie(options =>
      {
         options.Cookie.Name = "PW.Public.Session";
         options.LoginPath = "/Auth/Login";
         options.AccessDeniedPath = "/Error/Forbidden";
         options.Cookie.HttpOnly = true;
      });
      return services;
   }
}
