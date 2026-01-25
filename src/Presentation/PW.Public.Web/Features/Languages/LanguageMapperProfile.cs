using AutoMapper;
using PW.Application.Features.Localization.Dtos;
using PW.Public.Web.Features.Languages.ViewModels;

namespace PW.Public.Web.Features.Languages;

public class LanguageMapperProfile : Profile
{
   public LanguageMapperProfile()
   {
      CreateMap<LanguageLookupDto, LanguageSelectorItemViewModel>();
      CreateMap<LanguageDetailDto, LanguageSelectorItemViewModel>();
   }
}
