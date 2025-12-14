namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{

    public class GeneralSettingsLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }

        public string SiteTitle { get; set; }

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
    }
}
