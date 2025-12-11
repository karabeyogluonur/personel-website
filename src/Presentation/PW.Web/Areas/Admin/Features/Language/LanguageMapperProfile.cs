using AutoMapper;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language
{
    public class LanguageMapperProfile : Profile
    {
        public LanguageMapperProfile()
        {
            CreateMap<PW.Domain.Entities.Language, LanguageListItemViewModel>();
            CreateMap<LanguageCreateViewModel, Domain.Entities.Language>();

            CreateMap<Domain.Entities.Language, LanguageEditViewModel>()
                .ForMember(dest => dest.CurrentFlagFileName, opt => opt.MapFrom(src => src.FlagImageFileName))
                .ForMember(dest => dest.FlagImage, opt => opt.Ignore());

            CreateMap<LanguageEditViewModel, Domain.Entities.Language>()
                .ForMember(dest => dest.FlagImageFileName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
