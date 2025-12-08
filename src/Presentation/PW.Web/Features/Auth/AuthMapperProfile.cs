using AutoMapper;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth
{
    public class AuthMapperProfile : Profile
    {
        public AuthMapperProfile()
        {
            CreateMap<LoginViewModel, LoginDto>();
        }
    }
}
