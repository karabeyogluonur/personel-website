using AutoMapper;

using PW.Application.Models.Dtos.Localization;
using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Common;

public class CommonMapperProfile : Profile
{
    public CommonMapperProfile()
    {
        CreateMap<LanguageLookupDto, LanguageLookupViewModel>();
    }
}
