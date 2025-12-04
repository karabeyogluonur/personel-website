using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using PW.Web.Middlewares;

namespace PW.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            return app;
        }

        public static WebApplication ConfigureRoutes(this WebApplication app, IRouteConstraint cultureConstraint)
        {
            app.MapGet("/", context =>
            {
                var localizationOptions = context.RequestServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
                var defaultCulture = localizationOptions.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
                context.Response.Redirect($"/{defaultCulture}");
                return Task.CompletedTask;
            });

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "localized-default",
                pattern: "{culture}/{controller=Home}/{action=Index}/{id?}",
                constraints: new
                {
                    culture = cultureConstraint
                }
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            return app;
        }
    }
}
