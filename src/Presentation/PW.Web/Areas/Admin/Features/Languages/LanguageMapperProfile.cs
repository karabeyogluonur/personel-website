using AutoMapper;
using PW.Application.Features.Localization.Dtos;
using PW.Web.Areas.Admin.Features.Languages.ViewModels;

namespace PW.Web.Areas.Admin.Features.Languages;

public class LanguageMapperProfile : Profile
{
   public LanguageMapperProfile()
   {
      CreateMap<LanguageSummaryDto, LanguageListItemViewModel>();

      CreateMap<LanguageDetailDto, LanguageEditViewModel>()
          .ForMember(destination => destination.CurrentFlagFileName, options => options.MapFrom(source => source.FlagImageFileName))
          .ForMember(destination => destination.FlagImage, options => options.Ignore())
          .ForMember(destination => destination.RemoveFlagImage, options => options.Ignore());
   }
}
