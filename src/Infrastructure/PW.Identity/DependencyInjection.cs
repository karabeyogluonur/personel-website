using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Interfaces.Identity;
using PW.Identity.Contexts;
using PW.Identity.Entities;
using PW.Identity.Factories;
using PW.Identity.Services;

namespace PW.Identity;

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
      builder.Services.AddScoped<IIdentityAuthService, IdentityAuthService>();
      builder.Services.AddScoped<IIdentityRoleService, IdentityRoleService>();
      builder.Services.AddScoped<IIdentityUserService, ıdentityUserService>();

      #endregion

   }
}
