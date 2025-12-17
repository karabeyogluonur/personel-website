using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{
    public class GeneralSettingsViewModel
    {
        [Display(Name = "Site Title")]
        public string SiteTitle { get; set; } = string.Empty;

        [Display(Name = "Light Theme Logo")]
        public IFormFile? LightThemeLogoImage { get; set; }
        public string? LightThemeLogoPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveLightThemeLogo { get; set; }

        [Display(Name = "Dark Theme Logo")]
        public IFormFile? DarkThemeLogoImage { get; set; }
        public string? DarkThemeLogoPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveDarkThemeLogo { get; set; }

        [Display(Name = "Dark Theme Favicon")]
        public IFormFile? DarkThemeFaviconImage { get; set; }
        public string? DarkThemeFaviconPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveDarkThemeFavicon { get; set; }

        [Display(Name = "Light Theme Favicon")]
        public IFormFile? LightThemeFaviconImage { get; set; }
        public string? LightThemeFaviconPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveLightThemeFavicon { get; set; }

        public IList<GeneralSettingsLocalizedViewModel> Locales { get; set; } = new List<GeneralSettingsLocalizedViewModel>();

        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
