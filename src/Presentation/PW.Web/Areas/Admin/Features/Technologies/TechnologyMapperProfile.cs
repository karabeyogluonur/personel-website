using AutoMapper;
using PW.Application.Models.Dtos.Content;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technologies;

public class TechnologyMapperProfile : Profile
{
   public TechnologyMapperProfile()
   {
      CreateMap<TechnologySummaryDto, TechnologyListItemViewModel>();

      CreateMap<TechnologyDetailDto, TechnologyEditViewModel>()
          .ForMember(dest => dest.CurrentIconFileName, opt => opt.MapFrom(src => src.IconImageFileName))
          .ForMember(dest => dest.IconImage, opt => opt.Ignore())
          .ForMember(dest => dest.RemoveIconImage, opt => opt.Ignore())
          .ForMember(dest => dest.Translations, opt => opt.Ignore())
          .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
   }
}
