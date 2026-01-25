using PW.Web.Framework.Extensions;
using PW.Web.Framework.Middlewares;

namespace PW.Public.Web.Framework.Extensions;

public static class WebApplicationExtensions
{
   public static WebApplication ConfigurePublicPipeline(this WebApplication app)
   {
      app.ConfigureSharedPipelineStart();
      app.UseMiddleware<DynamicLocalizationMiddleware>();
      app.UseMiddleware<CultureRedirectMiddleware>();
      app.ConfigureSharedPipelineEnd();

      return app;
   }

   public static WebApplication ConfigurePublicRoutes(this WebApplication app)
   {
      app.MapControllerRoute(
          name: "defaultWithLanguage",
          pattern: "{culture}/{controller=Home}/{action=Index}/{id?}",
          constraints: new { culture = @"^[a-zA-Z]{2}$" }
      );

      app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}"
      );

      return app;
   }
}
