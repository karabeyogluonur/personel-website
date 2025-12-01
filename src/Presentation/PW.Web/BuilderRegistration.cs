using PW.Application.Common.Constants;

namespace PW.Web
{
    public static class BuilderRegistration
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseStaticFiles();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
        }

        public static void ConfigureRouting(this WebApplication app)
        {

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
