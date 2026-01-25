using AutoMapper;
using PW.Admin.Web.Features.Auth.ViewModels;
using PW.Application.Features.Auth.Dtos;

namespace PW.Admin.Web.Features.Auth;

public class AuthMapperProfile : Profile
{
   public AuthMapperProfile()
   {
      CreateMap<LoginViewModel, LoginDto>();
   }
}
