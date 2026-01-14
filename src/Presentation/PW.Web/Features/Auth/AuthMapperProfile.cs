using AutoMapper;
using PW.Application.Features.Auth.Dtos;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth;

public class AuthMapperProfile : Profile
{
   public AuthMapperProfile()
   {
      CreateMap<LoginViewModel, LoginDto>();
   }
}
