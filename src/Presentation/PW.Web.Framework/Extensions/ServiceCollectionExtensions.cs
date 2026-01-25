using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PW.Application;
using PW.Application.Common.Resources;
using PW.Caching;
using PW.Identity;
using PW.Persistence;
using PW.Storage;

namespace PW.Web.Framework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        builder.AddPersistenceServices();
        builder.AddIdentityServices();
        builder.AddApplicationServices();
        builder.AddStorageServices();
        builder.AddCachingServices();

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddSharedWebComponents(this IServiceCollection services, Assembly clientAssembly)
    {
        services.AddControllersWithViews(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResources));
            })
            .AddRazorRuntimeCompilation();

        services.AddLocalization();

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.AppendTrailingSlash = false;
        });

        services.AddAutoMapper(clientAssembly, Assembly.GetExecutingAssembly());

        return services;
    }
}
