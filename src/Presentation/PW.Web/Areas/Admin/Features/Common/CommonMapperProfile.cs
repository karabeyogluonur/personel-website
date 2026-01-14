using AutoMapper;
using PW.Application.Features.Localization.Dtos;
using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Common;

public class CommonMapperProfile : Profile
{
   public CommonMapperProfile()
   {
      CreateMap<LanguageLookupDto, LanguageLookupViewModel>();
   }
}
