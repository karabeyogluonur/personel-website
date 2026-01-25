using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PW.Web.Framework.Middlewares;

namespace PW.Web.Framework.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureSharedPipelineStart(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        return app;
    }

    public static WebApplication ConfigureSharedPipelineEnd(this WebApplication app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
