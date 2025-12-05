using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PW.Domain.Entities.Localization;
using PW.Persistence.Contexts;

namespace PW.Persistence
{
    public class DatabaseInitialiser
    {
        private readonly ILogger<DatabaseInitialiser> _logger;
        private readonly PWDbContext _context;

        public DatabaseInitialiser(ILogger<DatabaseInitialiser> logger, PWDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                _logger.LogInformation("Database initialisation and migration starting...");

                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();
                await SeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedLanguagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedLanguagesAsync()
        {
            if (!await _context.Languages.AnyAsync())
            {
                _logger.LogInformation("Seeding language data...");

                var languages = new List<Language>
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
                    },
                };

                await _context.Languages.AddRangeAsync(languages);
                await _context.SaveChangesAsync();
                _logger.LogInformation("2 languages successfully added to the database.");
            }
            else
            {
                _logger.LogInformation("Language table already contains data. Seeding skipped.");
            }
        }
    }
}
