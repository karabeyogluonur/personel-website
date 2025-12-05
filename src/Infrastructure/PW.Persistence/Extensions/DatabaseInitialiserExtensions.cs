using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PW.Persistence.Extensions
{
    public static class DatabaseInitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            DatabaseInitialiser initialiser;
            try
            {
                initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            await initialiser.InitialiseAsync();
        }
    }

}
