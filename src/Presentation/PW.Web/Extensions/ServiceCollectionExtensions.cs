using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using PW.Application;
using PW.Application.Common.Interfaces;
using PW.Identity;
using PW.Persistence;
using PW.Services;
using PW.Web.Areas.Admin.Features.Configuration.Services;
using PW.Web.Areas.Admin.Features.Language.Services;
using PW.Web.Areas.Admin.Features.User.Services;
using PW.Web.Features.Auth.Services;
using PW.Web.Services;
using System.Globalization;
using System.Reflection;

namespace PW.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.AddPersistenceServices();
            builder.AddIdentityServices();
            builder.AddApplicationServices();
            builder.AddServiceServices();
            services.AddWebInfrastructure();
            services.AddOrchestrators();
            services.ConfigureCustomCookie();

            services.AddDatabaseLocalizationServices();

            return services;
        }

        private static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddRazorRuntimeCompilation();

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddScoped<IWorkContext, WorkContext>();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddDatabaseLocalizationServices(this IServiceCollection services)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("tr")
                };
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture("en");
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider() { Options = options });
                options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            });

            return services;
        }

        private static IServiceCollection AddOrchestrators(this IServiceCollection services)
        {
            services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
            services.AddScoped<IUserOrchestrator, UserOrchestrator>();
            services.AddScoped<ILanguageOrchestrator, LanguageOrchestrator>();
            services.AddScoped<IProfileSettingsOrchestrator, ProfileSettingsOrchestrator>();
            return services;
        }

        private static IServiceCollection ConfigureCustomCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Error/403";
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });
            return services;
        }
    }
}
