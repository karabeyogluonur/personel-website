using AutoMapper;
using PW.Web.Areas.Admin.Features.Category.ViewModels;

namespace PW.Web.Areas.Admin.Features.Category
{
    public class CategoryMapperProfile : Profile
    {
        public CategoryMapperProfile()
        {
            CreateMap<Domain.Entities.Category, CategoryListItemViewModel>();

            CreateMap<CategoryCreateViewModel, Domain.Entities.Category>()
                .ForMember(dest => dest.CoverImageFileName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Domain.Entities.Category, CategoryEditViewModel>()
                .ForMember(dest => dest.CurrentCoverImageFileName, opt => opt.MapFrom(src => src.CoverImageFileName))
                .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.Locales, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());

            CreateMap<CategoryEditViewModel, Domain.Entities.Category>()
                .ForMember(dest => dest.CoverImageFileName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
