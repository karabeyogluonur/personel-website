using AutoMapper;
using PW.Application.Features.Localization.Dtos;
using PW.Admin.Web.Features.Common.Models;

namespace PW.Admin.Web.Features.Common;

public class CommonMapperProfile : Profile
{
   public CommonMapperProfile()
   {
      CreateMap<LanguageLookupDto, LanguageLookupViewModel>();
   }
}
