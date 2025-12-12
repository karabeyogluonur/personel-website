using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;
using PW.Web.Features.Languages.ViewModels;

namespace PW.Web.ViewComponents
{
    public class LanguageSelectorViewComponent : ViewComponent
    {
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IMapper _mapper;

        public LanguageSelectorViewComponent(ILanguageService languageService, IMapper mapper, IWorkContext workContext)
        {
            _languageService = languageService;
            _mapper = mapper;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var languages = await _languageService.GetAllPublishedLanguagesAsync();
            var currentLanguage = await _workContext.GetCurrentLanguageAsync();

            var model = new PublicLanguageSelectorViewModel
            {
                CurrentLanguage = _mapper.Map<PublicLanguageItemViewModel>(currentLanguage),
                AvailableLanguages = _mapper.Map<List<PublicLanguageItemViewModel>>(languages)
            };

            return View(model);
        }
    }
}


