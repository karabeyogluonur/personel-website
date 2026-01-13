namespace PW.Application.Models.Dtos.Configurations;

public class GeneralSettingsDto
{
    public string SiteTitle { get; set; } = string.Empty;
    public string? LightThemeLogoFileName { get; set; }
    public string? DarkThemeLogoFileName { get; set; }
    public string? LightThemeFaviconFileName { get; set; }
    public string? DarkThemeFaviconFileName { get; set; }

    public List<GeneralSettingsTranslationDto> Translations { get; set; } = new List<GeneralSettingsTranslationDto>();
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
    public Stream? LightThemeLogoStream { get; set; }
    public string? LightThemeLogoFileName { get; set; }
    public bool RemoveLightThemeLogo { get; set; }
    public Stream? DarkThemeLogoStream { get; set; }
    public string? DarkThemeLogoFileName { get; set; }
    public bool RemoveDarkThemeLogo { get; set; }
    public Stream? LightThemeFaviconStream { get; set; }
    public string? LightThemeFaviconFileName { get; set; }
    public bool RemoveLightThemeFavicon { get; set; }
    public Stream? DarkThemeFaviconStream { get; set; }
    public string? DarkThemeFaviconFileName { get; set; }
    public bool RemoveDarkThemeFavicon { get; set; }

    public List<GeneralSettingsTranslationUpdateDto> Translations { get; set; } = new List<GeneralSettingsTranslationUpdateDto>();
}

public class GeneralSettingsTranslationUpdateDto
{
    public int LanguageId { get; set; }
    public string SiteTitle { get; set; } = string.Empty;
    public Stream? LightThemeLogoStream { get; set; }
    public string? LightThemeLogoFileName { get; set; }
    public bool RemoveLightThemeLogo { get; set; }
    public Stream? DarkThemeLogoStream { get; set; }
    public string? DarkThemeLogoFileName { get; set; }
    public bool RemoveDarkThemeLogo { get; set; }
    public Stream? LightThemeFaviconStream { get; set; }
    public string? LightThemeFaviconFileName { get; set; }
    public bool RemoveLightThemeFavicon { get; set; }
    public Stream? DarkThemeFaviconStream { get; set; }
    public string? DarkThemeFaviconFileName { get; set; }
    public bool RemoveDarkThemeFavicon { get; set; }
}
