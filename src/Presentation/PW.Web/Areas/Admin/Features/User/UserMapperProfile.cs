using AutoMapper;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserDto, UserListItemViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.Ignore());

            CreateMap<UserDto, UserEditViewModel>()
                .ForMember(dest => dest.SelectedRoles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.AvailableRoles, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.ChangePassword, opt => opt.Ignore());

            CreateMap<UserEditViewModel, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.SelectedRoles))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserCreateViewModel, CreateUserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.SelectedRoles));
        }
    }
}
