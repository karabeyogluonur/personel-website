using AutoMapper;

using PW.Application.Models.Dtos.Localization;
using PW.Web.Features.Languages.ViewModels;

namespace PW.Web.Features.Languages;

public class LanguageMapperProfile : Profile
{
   public LanguageMapperProfile()
   {
      CreateMap<LanguageLookupDto, LanguageSelectorItemViewModel>();
      CreateMap<LanguageDetailDto, LanguageSelectorItemViewModel>();
   }
}
