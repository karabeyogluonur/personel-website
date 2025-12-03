using PW.Web.Mvc.Middlewares;

namespace PW.Web.Mvc
{
    public static class BuilderRegistration
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseStaticFiles();

            if (!app.Environment.IsDevelopment())
                app.UseHsts();
            else
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
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
