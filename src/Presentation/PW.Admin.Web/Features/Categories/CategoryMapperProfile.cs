using AutoMapper;
using PW.Application.Features.Categories.Dtos;
using PW.Admin.Web.Features.Categories.ViewModels;

namespace PW.Admin.Web.Features.Categories;

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
