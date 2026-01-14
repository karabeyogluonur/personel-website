using PW.Domain.Interfaces;

namespace PW.Domain.Configuration;

public class GeneralSettings : ISettings
{
   public string LightThemeLogoFileName { get; set; } = string.Empty;
   public string DarkThemeLogoFileName { get; set; } = string.Empty;
   public string LightThemeFaviconFileName { get; set; } = string.Empty;
   public string DarkThemeFaviconFileName { get; set; } = string.Empty;
   public string SiteTitle { get; set; } = string.Empty;
}
