using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Configurations.ViewModels;

public class GeneralSettingsTranslationViewModel
{
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;

    [Display(Name = "Site Title")]
    public string? SiteTitle { get; set; }

    [Display(Name = "Light Theme Logo")]
    public IFormFile? LightThemeLogoImage { get; set; }
    public string? LightThemeLogoPath { get; set; }
    public bool RemoveLightThemeLogo { get; set; }

    [Display(Name = "Dark Theme Logo")]
    public IFormFile? DarkThemeLogoImage { get; set; }
    public string? DarkThemeLogoPath { get; set; }
    public bool RemoveDarkThemeLogo { get; set; }

    [Display(Name = "Light Theme Favicon")]
    public IFormFile? LightThemeFaviconImage { get; set; }
    public string? LightThemeFaviconPath { get; set; }
    public bool RemoveLightThemeFavicon { get; set; }

    [Display(Name = "Dark Theme Favicon")]
    public IFormFile? DarkThemeFaviconImage { get; set; }
    public string? DarkThemeFaviconPath { get; set; }
    public bool RemoveDarkThemeFavicon { get; set; }
}
