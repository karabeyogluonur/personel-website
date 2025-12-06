using Microsoft.EntityFrameworkCore;
using PW.Domain.Entities.Localization;
using PW.Persistence.Contexts;
using PW.Application.Interfaces.Identity;
using PW.Application.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PW.Persistence
{
    public class DatabaseInitialiser
    {
        private readonly PWDbContext _context;
        private readonly IIdentityService _identityService;

        public DatabaseInitialiser(PWDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task InitialiseAsync()
        {
            await _context.Database.MigrateAsync();
            await SeedAsync();
        }

        public async Task SeedAsync()
        {
            await SeedLanguagesAsync();
        }

        private async Task SeedLanguagesAsync()
        {
            if (await _context.Languages.AnyAsync())
                return;

            var initialLanguages = new List<Language>
            {
                new Language
                {
                    Name = "English",
                    Code = "en",
                    FlagImageFileName = "en.svg",
                    IsDefault = true,
                    IsPublished = true,
                    DisplayOrder = 1
                },
                new Language
                {
                    Name = "Türkçe",
                    Code = "tr",
                    FlagImageFileName = "tr.svg",
                    IsDefault = false,
                    IsPublished = true,
                    DisplayOrder = 1
                }
            };

            await _context.Languages.AddRangeAsync(initialLanguages);
            await _context.SaveChangesAsync();
        }

    }


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
