using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using PW.Application;      // AddApplicationServices için
using PW.Application.Common.Interfaces;
using PW.Identity;         // AddIdentityServices için
using PW.Persistence;      // AddPersistenceServices için
using PW.Services;         // AddServiceServices için
using PW.Web.Areas.Admin.Features.Language.Services;
using PW.Web.Areas.Admin.Features.User.Services;
using PW.Web.Features.Auth.Services;
using PW.Web.Services;     // WebWorkContext için
using System.Reflection;

namespace PW.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Ana Metot: Program.cs bunu çağıracak
        public static IServiceCollection AddProjectServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.AddPersistenceServices();
            builder.AddIdentityServices();
            builder.AddApplicationServices();
            builder.AddServiceServices();
            services.AddWebInfrastructure();
            services.AddOrchestrators();
            services.ConfigureCustomCookie();

            return services;
        }

        private static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddRazorRuntimeCompilation();

            services.AddLocalization();
            services.AddHttpContextAccessor();

            services.AddScoped<IWorkContext, WorkContext>();

            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }

        private static IServiceCollection AddOrchestrators(this IServiceCollection services)
        {
            services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
            services.AddScoped<IUserOrchestrator, UserOrchestrator>();
            services.AddScoped<ILanguageOrchestrator, LanguageOrchestrator>();
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
