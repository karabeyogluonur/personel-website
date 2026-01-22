using AutoMapper;
using PW.Application.Features.Tags.Dtos;
using PW.Web.Areas.Admin.Features.Tags.ViewModels;

namespace PW.Web.Areas.Admin.Features.Tags;

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
