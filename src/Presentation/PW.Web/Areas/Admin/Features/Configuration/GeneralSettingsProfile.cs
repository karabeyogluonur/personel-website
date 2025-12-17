using AutoMapper;
using PW.Domain.Configuration;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration
{
    public class GeneralSettingsProfile : Profile
    {
        public GeneralSettingsProfile()
        {
            CreateMap<GeneralSettings, GeneralSettingsViewModel>()
                .ForMember(dest => dest.DarkThemeFaviconPath, opt => opt.MapFrom(src => src.DarkThemeFaviconFileName))
                .ForMember(dest => dest.LightThemeFaviconPath, opt => opt.MapFrom(src => src.LightThemeFaviconFileName))
                .ForMember(dest => dest.DarkThemeLogoPath, opt => opt.MapFrom(src => src.DarkThemeLogoFileName))
                .ForMember(dest => dest.LightThemeLogoPath, opt => opt.MapFrom(src => src.LightThemeLogoFileName))
                .ForMember(dest => dest.DarkThemeFaviconImage, opt => opt.Ignore())
                .ForMember(dest => dest.LightThemeFaviconImage, opt => opt.Ignore())
                .ForMember(dest => dest.DarkThemeLogoImage, opt => opt.Ignore())
                .ForMember(dest => dest.LightThemeLogoImage, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveDarkThemeFavicon, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveDarkThemeLogo, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveLightThemeFavicon, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveLightThemeLogo, opt => opt.Ignore())
                .ForMember(dest => dest.Locales, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
        }
    }
}
