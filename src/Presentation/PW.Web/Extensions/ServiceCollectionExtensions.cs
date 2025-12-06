using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Razor;
using PW.Application;
using PW.Identity;
using PW.Persistence;
using PW.Services;
using PW.Web.Features.Auth.Services;

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

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
        public static IServiceCollection AddOrchestratorServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
            return services;
        }

        public static IServiceCollection ConfigureCustomApplicationCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Error/403";
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
            });
            return services;
        }

        public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.AddApplicationServices();
            builder.AddPersistenceServices();
            builder.AddServiceServices();
            builder.AddIdentityServices();

            return services;
        }
    }
}
