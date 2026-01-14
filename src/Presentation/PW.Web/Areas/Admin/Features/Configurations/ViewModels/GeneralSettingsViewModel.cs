using System.ComponentModel.DataAnnotations;

using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Configurations.ViewModels;

public class GeneralSettingsViewModel
{
   [Display(Name = "Site Title (Default)")]
   public string SiteTitle { get; set; } = string.Empty;

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
   public IList<GeneralSettingsTranslationViewModel> Translations { get; set; } = new List<GeneralSettingsTranslationViewModel>();

   public IList<LanguageLookupViewModel> AvailableLanguages { get; set; } = new List<LanguageLookupViewModel>();
}
