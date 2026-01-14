using AutoMapper;
using PW.Web.Areas.Admin.Features.Users.ViewModels;
using static PW.Application.Features.Users.Dtos.UserDto;

namespace PW.Web.Areas.Admin.Features.Users;

public class UserMapperProfile : Profile
{
   public UserMapperProfile()
   {
      CreateMap<UserCreateViewModel, UserCreateDto>();

      CreateMap<UserEditViewModel, UserUpdateDto>()
          .ForMember(dest => dest.IsPasswordChangeRequested, opt => opt.MapFrom(src => src.ChangePassword))
          .ForMember(dest => dest.NewPassword, opt => opt.MapFrom(src => src.Password))
          .ForMember(dest => dest.SelectedRoles, opt => opt.MapFrom(src => src.SelectedRoles));

      CreateMap<UserDetailDto, UserEditViewModel>();
      CreateMap<UserSummaryDto, UserListItemViewModel>();
   }
}
