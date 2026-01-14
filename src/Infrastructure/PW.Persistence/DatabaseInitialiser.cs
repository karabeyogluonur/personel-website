using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PW.Application.Common.Extensions;
using PW.Domain.Configuration;
using PW.Domain.Entities;
using PW.Persistence.Contexts;

namespace PW.Persistence;

public class DatabaseInitialiser
{
   private readonly PWDbContext _context;

   public DatabaseInitialiser(PWDbContext context)
   {
      _context = context;
   }

   public async Task InitialiseAsync()
   {
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

      List<Language> initialLanguages = new List<Language>
        {
            new Language { Name = "Türkçe", Code = "tr", FlagImageFileName = "tr.svg", IsDefault = false, IsPublished = true, DisplayOrder = 1,CreatedAt=DateTime.UtcNow },
            new Language { Name = "English", Code = "en", FlagImageFileName = "en.svg", IsDefault = true, IsPublished = true, DisplayOrder = 2, CreatedAt=DateTime.UtcNow }
        };

      await _context.Languages.AddRangeAsync(initialLanguages);
      await _context.SaveChangesAsync();
   }

   private async Task SeedSettingsAsync()
   {
      Language? trLang = await _context.Languages.FirstOrDefaultAsync(lang => lang.Code == "tr");
      Language? enLang = await _context.Languages.FirstOrDefaultAsync(lang => lang.Code == "en");

      if (trLang == null || enLang == null) return;

      ProfileSettings defaultProfileSettings = new ProfileSettings
      {
         FirstName = "Onur",
         LastName = "Karabeyoğlu",
         JobTitle = "Software Developer",
         Biography = "Merhaba, ben Onur. .NET teknolojileri üzerine uzmanlaşmış bir yazılım geliştiriciyim.",
         AvatarFileName = string.Empty,
         CoverFileName = string.Empty
      };

      await SeedSettingsHelperAsync(defaultProfileSettings);

      GeneralSettings defaultGeneralSettings = new GeneralSettings
      {
         SiteTitle = "Kişisel Portfolyom",
         LightThemeLogoFileName = string.Empty,
         DarkThemeLogoFileName = string.Empty,
         LightThemeFaviconFileName = string.Empty,
         DarkThemeFaviconFileName = string.Empty
      };

      await SeedSettingsHelperAsync(defaultGeneralSettings);
      await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.FirstName), enLang.Id, "Onur");
      await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.LastName), enLang.Id, "Karabeyoglu");
      await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.JobTitle), enLang.Id, "Software Developer");
      await AddSettingTranslationAsync<ProfileSettings>(nameof(ProfileSettings.Biography), enLang.Id, "Hello, I am Onur. I am a software developer specializing in .NET technologies.");
      await AddSettingTranslationAsync<GeneralSettings>(nameof(GeneralSettings.SiteTitle), enLang.Id, "My Personal Portfolio");
      await _context.SaveChangesAsync();
   }

   private async Task AddSettingTranslationAsync<T>(string propName, int languageId, string translationValue) where T : class
   {
      string key = typeof(T).BuildSettingKey(propName);

      Setting? setting = await _context.Settings
          .Include(setting => setting.Translations)
          .FirstOrDefaultAsync(setting => setting.Name == key);

      if (setting == null) return;

      bool exists = setting.Translations.Any(translation => translation.LanguageId == languageId);

      if (!exists)
      {
         setting.Translations.Add(new SettingTranslation
         {
            LanguageId = languageId,
            Value = translationValue
         });
      }
   }

   private async Task SeedSettingsHelperAsync<T>(T settings) where T : class
   {
      foreach (PropertyInfo prop in typeof(T).GetProperties())
      {
         if (!prop.CanRead) continue;

         string key = typeof(T).BuildSettingKey(prop.Name);

         bool exists = await _context.Settings.AnyAsync(setting => setting.Name == key);

         if (!exists)
         {
            object? value = prop.GetValue(settings);
            string valueStr = value?.ToInvariantString() ?? string.Empty;

            Setting newSetting = new Setting
            {
               Name = key,
               Value = valueStr,
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
      using IServiceScope scope = app.Services.CreateScope();

      DatabaseInitialiser initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();
      await initialiser.InitialiseAsync();
   }
}
