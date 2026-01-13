using PW.Application.Models.Dtos.Common;

namespace PW.Application.Models.Dtos.Configurations;

public class GeneralSettingsDto
{
   public string SiteTitle { get; set; } = string.Empty;
   public string? LightThemeLogoFileName { get; set; }
   public string? DarkThemeLogoFileName { get; set; }
   public string? LightThemeFaviconFileName { get; set; }
   public string? DarkThemeFaviconFileName { get; set; }

   public List<GeneralSettingsTranslationDto> Translations { get; set; } = new();
}

public class GeneralSettingsTranslationDto
{
   public int LanguageId { get; set; }
   public string SiteTitle { get; set; } = string.Empty;
   public string? LightThemeLogoFileName { get; set; }
   public string? DarkThemeLogoFileName { get; set; }
   public string? LightThemeFaviconFileName { get; set; }
   public string? DarkThemeFaviconFileName { get; set; }
}

public class GeneralSettingsUpdateDto
{
   public string SiteTitle { get; set; } = string.Empty;
   public FileUploadDto LightThemeLogo { get; set; } = new();
   public FileUploadDto DarkThemeLogo { get; set; } = new();
   public FileUploadDto LightThemeFavicon { get; set; } = new();
   public FileUploadDto DarkThemeFavicon { get; set; } = new();

   public List<GeneralSettingsTranslationUpdateDto> Translations { get; set; } = new();
}

public class GeneralSettingsTranslationUpdateDto
{
   public int LanguageId { get; set; }
   public string SiteTitle { get; set; } = string.Empty;
   public FileUploadDto LightThemeLogo { get; set; } = new();
   public FileUploadDto DarkThemeLogo { get; set; } = new();
   public FileUploadDto LightThemeFavicon { get; set; } = new();
   public FileUploadDto DarkThemeFavicon { get; set; } = new();
}
