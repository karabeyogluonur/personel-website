using AutoMapper;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration
{
    public class ProfileSettingsProfile : Profile
    {
        public ProfileSettingsProfile()
        {
            CreateMap<Domain.Configuration.ProfileSettings, ProfileSettingsViewModel>()
                .ForMember(dest => dest.AvatarPath, opt => opt.MapFrom(src => src.AvatarFileName))
                .ForMember(dest => dest.CoverPath, opt => opt.MapFrom(src => src.CoverFileName))
                .ForMember(dest => dest.AvatarImage, opt => opt.Ignore())
                .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveAvatar, opt => opt.Ignore())
                .ForMember(dest => dest.RemoveCover, opt => opt.Ignore())
                .ForMember(dest => dest.Locales, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
        }
    }
}
