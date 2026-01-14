using AutoMapper;
using PW.Application.Features.Configuration.Dtos;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration;

public class ProfileSettingsMapperProfile : Profile
{
   public ProfileSettingsMapperProfile()
   {
      CreateMap<ProfileSettingsTranslationDto, ProfileSettingsTranslationViewModel>()
          .ForMember(dest => dest.AvatarPath, opt => opt.MapFrom(src => src.AvatarFileName))
          .ForMember(dest => dest.CoverPath, opt => opt.MapFrom(src => src.CoverFileName))
          .ForMember(dest => dest.AvatarImage, opt => opt.Ignore())
          .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveAvatar, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveCover, opt => opt.Ignore())
          .ForMember(dest => dest.LanguageCode, opt => opt.Ignore());

      CreateMap<ProfileSettingsDto, ProfileSettingsViewModel>()
          .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
          .ForMember(dest => dest.AvatarPath, opt => opt.MapFrom(src => src.AvatarFileName))
          .ForMember(dest => dest.CoverPath, opt => opt.MapFrom(src => src.CoverFileName))
          .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore())
          .ForMember(dest => dest.AvatarImage, opt => opt.Ignore())
          .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveAvatar, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveCover, opt => opt.Ignore());
   }
}
