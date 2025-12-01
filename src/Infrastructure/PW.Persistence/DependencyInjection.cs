using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PW.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PW.Domain.Common;
using PW.Persistence.Interceptors;
using TM.Infrastructure.Persistence.Interceptors;

namespace PW.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistenceServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton(TimeProvider.System);

            #region Contexts

            builder.Services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
            builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

            builder.Services.AddDbContext<PWDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion
        }
    }
}
