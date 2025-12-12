using AutoMapper;
using PW.Domain.Entities;
using PW.Web.Features.Languages.ViewModels;

namespace PW.Web.Features.Languages
{
    public class PublicLanguageMapperProfile : Profile
    {
        public PublicLanguageMapperProfile()
        {
            CreateMap<Language, PublicLanguageItemViewModel>();
        }
    }
}
