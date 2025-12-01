using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PW.Persistence.Contexts;

namespace PW.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistenceServices(this IHostApplicationBuilder builder)
        {
            #region Contexts

            var test = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<PWDbContext>((sp, options) =>
            {
                options.UseNpgsql(test);
            });

            #endregion
        }
    }
}
