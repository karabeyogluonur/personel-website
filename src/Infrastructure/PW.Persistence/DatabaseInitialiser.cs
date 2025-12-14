using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PW.Domain.Configuration;
using PW.Domain.Entities;
using PW.Persistence.Contexts;
using PW.Application.Common.Extensions;

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

            var defaultGeneralSettings = new GeneralSettings
            {
                SiteTitle = "My Portfolio",
                LightThemeLogoFileName = "",
                DarkThemeLogoFileName = "",
                LightThemeFaviconFileName = "",
                DarkThemeFaviconFileName = ""
            };

            await SeedSettingsHelperAsync(defaultGeneralSettings);

            if (trLang != null && enLang != null)
            {
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.FirstName), enLang.Id, "Onur");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.LastName), enLang.Id, "Karabeyoglu");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.JobTitle), enLang.Id, "Software Developer");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.Biography), enLang.Id, "Hello, I am Onur. I am a software developer specializing in .NET technologies.");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.AvatarFileName), enLang.Id, "");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.CoverFileName), enLang.Id, "");

                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.SiteTitle), enLang.Id, "My Personal Portfolio");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.LightThemeLogoFileName), enLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.DarkThemeLogoFileName), enLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.LightThemeFaviconFileName), enLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.DarkThemeFaviconFileName), enLang.Id, "");

                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.FirstName), trLang.Id, "Onur");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.LastName), trLang.Id, "Karabeyoğlu");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.JobTitle), trLang.Id, "Yazılım Geliştirici");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.Biography), trLang.Id, "Merhaba, ben Onur. .NET teknolojileri üzerine uzmanlaşmış bir yazılım geliştiriciyim.");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.AvatarFileName), trLang.Id, "");
                await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.CoverFileName), trLang.Id, "");

                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.SiteTitle), trLang.Id, "Kişisel Portfolyom");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.LightThemeLogoFileName), trLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.DarkThemeLogoFileName), trLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.LightThemeFaviconFileName), trLang.Id, "");
                await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.DarkThemeFaviconFileName), trLang.Id, "");
            }

            await _context.SaveChangesAsync();
        }

        private async Task AddSettingTranslationAsync<T>(string propName, int languageId, string translation)
        {
            string key = typeof(T).BuildSettingKey(propName);

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
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead) continue;

                string key = typeof(T).BuildSettingKey(prop.Name);

                bool exists = await _context.Settings.AnyAsync(x => x.Name == key);

                if (!exists)
                {
                    dynamic value = prop.GetValue(settings);

                    string valueStr = ((object)value).ToInvariantString();

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
            try
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();
                await initialiser.InitialiseAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
