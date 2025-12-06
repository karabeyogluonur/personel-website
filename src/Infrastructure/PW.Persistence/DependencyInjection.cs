using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PW.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PW.Persistence.Interceptors;
using TM.Infrastructure.Persistence.Interceptors;
using PW.Persistence.Repositories;
using PW.Application.Interfaces.Repositories;
using PW.Identity.Entities;
using PW.Application.Common.Interfaces;

namespace PW.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistenceServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton(TimeProvider.System);

            #region Contexts
            builder.Services.AddScoped<IPWDbContext>(provider => provider.GetRequiredService<PWDbContext>());
            builder.Services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
            builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

            builder.Services.AddDbContext<PWDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<DatabaseInitialiser>();

            #endregion

        }
    }
}
