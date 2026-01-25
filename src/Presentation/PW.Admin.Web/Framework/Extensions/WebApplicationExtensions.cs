using PW.Web.Framework.Extensions;

namespace PW.Admin.Web.Framework.Extensions;

public static class WebApplicationExtensions
{
   public static WebApplication ConfigureAdminPipeline(this WebApplication app)
   {
      app.ConfigureSharedPipelineStart();
      //
      app.ConfigureSharedPipelineEnd();

      return app;
   }

   public static WebApplication ConfigureAdminRoutes(this WebApplication app)
   {
      app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}"
      );

      return app;
   }
}
