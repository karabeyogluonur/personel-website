using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PW.Web.Framework.Middlewares;

namespace PW.Web.Framework.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureSharedPipelineStart(this WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;

        if (env.IsDevelopment())
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

        if (env.IsDevelopment())
        {
            string frameworkPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "PW.Web.Framework", "wwwroot");

            if (Directory.Exists(frameworkPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(frameworkPath),
                });
            }
        }

        ConfigureUploadsDirectory(app);

        return app;
    }

    public static WebApplication ConfigureSharedPipelineEnd(this WebApplication app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static void ConfigureUploadsDirectory(WebApplication app)
    {
        IConfigurationSection storageConfig = app.Configuration.GetSection("Storage");
        string? storagePath = storageConfig["Path"];

        if (string.IsNullOrEmpty(storagePath))
            return;

        string requestPath = storageConfig["RequestPath"] ?? "/uploads";
        bool useRelative = storageConfig.GetValue<bool>("UseRelativePath");
        string fullPath;

        if (useRelative)
            fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), storagePath));
        else
            fullPath = storagePath;

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fullPath),
            RequestPath = requestPath
        });
    }
}
