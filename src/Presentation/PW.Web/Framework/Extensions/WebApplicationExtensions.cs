using PW.Web.Framework.Middlewares;

namespace PW.Web.Framework.Extensions;

public static class WebApplicationExtensions
{
   public static WebApplication ConfigurePipeline(this WebApplication app)
   {
      if (app.Environment.IsDevelopment())
      {
         app.UseDeveloperExceptionPage();
      }
      else
      {
         app.UseMiddleware<GlobalExceptionMiddleware>();
         app.UseHsts();
      }

      app.UseStatusCodePagesWithReExecute("/Error/{0}");
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseMiddleware<DynamicLocalizationMiddleware>();
      app.UseMiddleware<CultureRedirectMiddleware>();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      return app;
   }

   public static WebApplication ConfigureRoutes(this WebApplication app)
   {

      app.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
      );

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
