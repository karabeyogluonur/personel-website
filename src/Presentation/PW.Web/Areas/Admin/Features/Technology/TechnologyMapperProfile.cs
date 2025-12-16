using AutoMapper;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology
{
    public class TechnologyMapperProfile : Profile
    {
        public TechnologyMapperProfile()
        {
            CreateMap<Domain.Entities.Technology, TechnologyListItemViewModel>();

            CreateMap<TechnologyCreateViewModel, Domain.Entities.Technology>()
                .ForMember(dest => dest.IconImageFileName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Domain.Entities.Technology, TechnologyEditViewModel>()
                .ForMember(dest => dest.CurrentIconFileName, opt => opt.MapFrom(src => src.IconImageFileName))
                .ForMember(dest => dest.IconImage, opt => opt.Ignore())
                .ForMember(dest => dest.Locales, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());

            CreateMap<TechnologyEditViewModel, Domain.Entities.Technology>()
                .ForMember(dest => dest.IconImageFileName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
