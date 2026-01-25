using AutoMapper;
using PW.Application.Features.Tags.Dtos;
using PW.Admin.Web.Features.Tags.ViewModels;

namespace PW.Admin.Web.Features.Tags;

public class TagMapperProfile : Profile
{
    public TagMapperProfile()
    {
        CreateMap<TagSummaryDto, TagListItemViewModel>();

        CreateMap<TagDetailDto, TagEditViewModel>()
            .ForMember(dest => dest.Translations, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
    }
}
