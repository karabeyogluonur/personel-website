using PW.Application.Interfaces.Localization;
using PW.Web.Middlewares;

namespace PW.Web.Extensions
{
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
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static WebApplication ConfigureRoutes(this WebApplication app)
        {
            app.MapGet("/", async context =>
            {
                var languageService = context.RequestServices.GetRequiredService<ILanguageService>();

                var defaultLang = await languageService.GetDefaultLanguageAsync();

                var code = defaultLang?.Code ?? "en";
                context.Response.Redirect($"/{code}", permanent: false);
            });

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
}
