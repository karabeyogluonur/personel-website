using System.ComponentModel;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{
    public class GeneralSettingsViewModel
    {
        [DisplayName("Site Title")]
        public string SiteTitle { get; set; } = string.Empty;

        public IFormFile? LightThemeLogoImage { get; set; }
        public string? LightThemeLogoPath { get; set; }
        public bool RemoveLightThemeLogo { get; set; }

        public IFormFile? DarkThemeLogoImage { get; set; }
        public string? DarkThemeLogoPath { get; set; }
        public bool RemoveDarkThemeLogo { get; set; }

        public IFormFile? DarkThemeFaviconImage { get; set; }
        public string? DarkThemeFaviconPath { get; set; }
        public bool RemoveDarkThemeFavicon { get; set; }

        public IFormFile? LightThemeFaviconImage { get; set; }
        public string? LightThemeFaviconPath { get; set; }
        public bool RemoveLightThemeFavicon { get; set; }

        public IList<GeneralSettingsLocalizedViewModel> Locales { get; set; } = new List<GeneralSettingsLocalizedViewModel>();

        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
