using AutoMapper;

using PW.Application.Models.Dtos.Configurations;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration;

public class GeneralSettingsMapperProfile : Profile
{
   public GeneralSettingsMapperProfile()
   {
      CreateMap<GeneralSettingsTranslationDto, GeneralSettingsTranslationViewModel>()
          .ForMember(dest => dest.LightThemeLogoPath, opt => opt.MapFrom(src => src.LightThemeLogoFileName))
          .ForMember(dest => dest.DarkThemeLogoPath, opt => opt.MapFrom(src => src.DarkThemeLogoFileName))
          .ForMember(dest => dest.LightThemeFaviconPath, opt => opt.MapFrom(src => src.LightThemeFaviconFileName))
          .ForMember(dest => dest.DarkThemeFaviconPath, opt => opt.MapFrom(src => src.DarkThemeFaviconFileName))
          .ForMember(dest => dest.LightThemeLogoImage, opt => opt.Ignore())
          .ForMember(dest => dest.DarkThemeLogoImage, opt => opt.Ignore())
          .ForMember(dest => dest.LightThemeFaviconImage, opt => opt.Ignore())
          .ForMember(dest => dest.DarkThemeFaviconImage, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveLightThemeLogo, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveDarkThemeLogo, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveLightThemeFavicon, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveDarkThemeFavicon, opt => opt.Ignore());

      CreateMap<GeneralSettingsDto, GeneralSettingsViewModel>()
          .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
          .ForMember(dest => dest.LightThemeLogoPath, opt => opt.MapFrom(src => src.LightThemeLogoFileName))
          .ForMember(dest => dest.DarkThemeLogoPath, opt => opt.MapFrom(src => src.DarkThemeLogoFileName))
          .ForMember(dest => dest.LightThemeFaviconPath, opt => opt.MapFrom(src => src.LightThemeFaviconFileName))
          .ForMember(dest => dest.DarkThemeFaviconPath, opt => opt.MapFrom(src => src.DarkThemeFaviconFileName))
          .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore())
          .ForMember(dest => dest.LightThemeLogoImage, opt => opt.Ignore())
          .ForMember(dest => dest.DarkThemeLogoImage, opt => opt.Ignore())
          .ForMember(dest => dest.LightThemeFaviconImage, opt => opt.Ignore())
          .ForMember(dest => dest.DarkThemeFaviconImage, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveLightThemeLogo, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveDarkThemeLogo, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveLightThemeFavicon, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveDarkThemeFavicon, opt => opt.Ignore());
   }
}
