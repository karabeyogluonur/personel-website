using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PW.Domain.Configuration;
using PW.Domain.Entities;
using PW.Persistence.Contexts;
using System.ComponentModel;

namespace PW.Persistence
{
    public class DatabaseInitialiser
    {
        private readonly PWDbContext _context;

        public DatabaseInitialiser(PWDbContext context)
        {
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();
            await SeedAsync();
        }

        public async Task SeedAsync()
        {
            await SeedLanguagesAsync();
            await SeedSettingsAsync();
        }
        private async Task SeedLanguagesAsync()
        {
            if (await _context.Languages.AnyAsync())
                return;

            var initialLanguages = new List<Language>
            {
                new Language { Name = "English", Code = "en", FlagImageFileName = "en.svg", IsDefault = true, IsPublished = true, DisplayOrder = 1 },
                new Language { Name = "Türkçe", Code = "tr", FlagImageFileName = "tr.svg", IsDefault = false, IsPublished = true, DisplayOrder = 2 }
            };

            await _context.Languages.AddRangeAsync(initialLanguages);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSettingsAsync()
        {
            var defaultProfileSettings = new ProfileSettings
            {
                FirstName = "Onur",
                LastName = "Karabeyoğlu",
                Biography = "Buraya kısa biyografinizi yazınız...",
            };
            await SeedSettingsHelperAsync(defaultProfileSettings);
        }

        private async Task SeedSettingsHelperAsync<T>(T settings) where T : class
        {
            string className = typeof(T).Name;

            if (className.EndsWith("Settings"))
                className = className.Substring(0, className.Length - "Settings".Length);

            string prefix = className;

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead) continue;

                string key = $"{prefix}.{prop.Name}";

                bool exists = await _context.Settings.AnyAsync(x => x.Name == key);

                if (!exists)
                {
                    dynamic value = prop.GetValue(settings);
                    string valueStr = "";

                    var converter = TypeDescriptor.GetConverter(prop.PropertyType);

                    if (value != null)
                        valueStr = converter.ConvertToInvariantString(value);

                    var newSetting = new Setting
                    {
                        Name = key,
                        Value = valueStr,
                        IsPublic = true
                    };

                    await _context.Settings.AddAsync(newSetting);
                }
            }
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
