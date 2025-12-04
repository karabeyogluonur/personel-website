using Microsoft.AspNetCore.Mvc.Razor;
using PW.Application;
using PW.Persistence;
using PW.Services;

namespace PW.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebLayerServices(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddRazorRuntimeCompilation();

            services.AddLocalization();

            return services;
        }

        public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.AddApplicationServices();
            builder.AddPersistenceServices();
            builder.AddServiceServices();

            return services;
        }
    }
}
