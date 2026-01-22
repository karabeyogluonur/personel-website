using AutoMapper;
using PW.Application.Features.Categories.Dtos;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories;

public class CategoryMapperProfile : Profile
{
    public CategoryMapperProfile()
    {
        CreateMap<CategorySummaryDto, CategoryListItemViewModel>();

        CreateMap<CategoryDetailDto, CategoryEditViewModel>()
            .ForMember(dest => dest.Translations, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
    }
}
