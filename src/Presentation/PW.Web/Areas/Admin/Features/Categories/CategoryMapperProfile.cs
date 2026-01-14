using AutoMapper;

using PW.Application.Models.Dtos.Content;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories;

public class CategoryMapperProfile : Profile
{
   public CategoryMapperProfile()
   {
      CreateMap<CategorySummaryDto, CategoryListItemViewModel>()
          .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

      CreateMap<CategoryDetailDto, CategoryEditViewModel>()
          .ForMember(dest => dest.Translations, opt => opt.Ignore())
          .ForMember(dest => dest.AvailableLanguages, opt => opt.Ignore());
   }
}
