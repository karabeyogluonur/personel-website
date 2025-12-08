using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PW.Identity.Entities;
using PW.Identity.Contexts;
using PW.Application.Interfaces.Identity;
using PW.Identity.Services;
using PW.Identity.Factories;
using Microsoft.AspNetCore.Identity;

namespace PW.Identity
{
    public static class DependencyInjection
    {
        public static void AddIdentityServices(this IHostApplicationBuilder builder)
        {

            #region Identity

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<AuthDbContext>();

            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            #endregion

            #region Contexts

            builder.Services.AddDbContext<AuthDbContext>((sp, options) =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddScoped<IdentityInitialiser>();

            #endregion

            #region Services

            builder.Services.AddScoped<IIdentityService, IdentityService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            #endregion

        }
    }
}
