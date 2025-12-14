using PW.Domain.Interfaces;

namespace PW.Domain.Configuration
{
    public class GeneralSettings : ISettings
    {
        public string LightThemeLogoFileName { get; set; }
        public string DarkThemeLogoFileName { get; set; }
        public string LightThemeFaviconFileName { get; set; }
        public string DarkThemeFaviconFileName { get; set; }
        public string SiteTitle { get; set; }
    }
}
