using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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
                new Language { Name = "English", Code = "en", FlagImageFileName = "en.svg", IsDefault = false, IsPublished = true, DisplayOrder = 2 },
                new Language { Name = "Türkçe", Code = "tr", FlagImageFileName = "tr.svg", IsDefault = true, IsPublished = true, DisplayOrder = 1 }
            };

            await _context.Languages.AddRangeAsync(initialLanguages);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSettingsAsync()
        {
            var trLang = await _context.Languages.FirstOrDefaultAsync(x => x.Code == "tr");
            var enLang = await _context.Languages.FirstOrDefaultAsync(x => x.Code == "en");

            var defaultProfileSettings = new ProfileSettings
            {
                FirstName = "Onur",
                LastName = "Karabeyoğlu",
                JobTitle = "Software Developer",
                Biography = "Hello, I am Onur. I am a software developer specializing in .NET technologies.",
                AvatarFileName = "",
                CoverFileName = ""
            };

            await SeedSettingsHelperAsync(defaultProfileSettings);

            if (trLang != null && enLang != null)
            {

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.FirstName),
                    languageId: enLang.Id,
                    translation: "Onur");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.LastName),
                    languageId: enLang.Id,
                    translation: "Karabeyoglu");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.JobTitle),
                    languageId: enLang.Id,
                    translation: "Software Developer");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.Biography),
                    languageId: enLang.Id,
                    translation: "Hello, I am Onur. I am a software developer specializing in .NET technologies.");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.AvatarFileName),
                    languageId: enLang.Id,
                    translation: "");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.CoverFileName),
                    languageId: enLang.Id,
                    translation: "");


                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.FirstName),
                    languageId: trLang.Id,
                    translation: "Onur");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.LastName),
                    languageId: trLang.Id,
                    translation: "Karabeyoğlu");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.JobTitle),
                    languageId: trLang.Id,
                    translation: "Yazılım Geliştirici");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.Biography),
                    languageId: trLang.Id,
                    translation: "Merhaba, ben Onur. .NET teknolojileri üzerine uzmanlaşmış bir yazılım geliştiriciyim.");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.AvatarFileName),
                    languageId: trLang.Id,
                    translation: "");

                await AddSettingTranslationAsync<ProfileSettings>(
                    propName: nameof(ProfileSettings.CoverFileName),
                    languageId: trLang.Id,
                    translation: "");
            }

            await _context.SaveChangesAsync();
        }
        private async Task AddSettingTranslationAsync<T>(string propName, int languageId, string translation)
        {
            string className = typeof(T).Name;
            if (className.EndsWith("Settings"))
                className = className.Substring(0, className.Length - "Settings".Length);

            string key = $"{className}.{propName}";

            var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Name == key);

            if (setting == null) return;

            bool exists = await _context.LocalizedProperties.AnyAsync(x =>
                x.EntityId == setting.Id &&
                x.LanguageId == languageId &&
                x.LocaleKeyGroup == "Setting" &&
                x.LocaleKey == "Value");

            if (!exists)
            {
                var localeProp = new LocalizedProperty
                {
                    EntityId = setting.Id,
                    LanguageId = languageId,
                    LocaleKeyGroup = "Setting",
                    LocaleKey = "Value",
                    LocaleValue = translation
                };

                await _context.LocalizedProperties.AddAsync(localeProp);
            }
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
