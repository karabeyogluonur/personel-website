using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;
using PW.Application.Models.Dtos.Localization;
using PW.Web.Features.Languages.ViewModels;

namespace PW.Web.ViewComponents;

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
      IList<LanguageLookupDto> languages = await _languageService.GetLanguagesLookupAsync();
      LanguageDetailDto currentLanguage = await _workContext.GetCurrentLanguageAsync();

      LanguageSelectorViewModel languageSelectorViewModel = new LanguageSelectorViewModel
      {
         CurrentLanguage = _mapper.Map<LanguageSelectorItemViewModel>(currentLanguage),
         AvailableLanguages = _mapper.Map<List<LanguageSelectorItemViewModel>>(languages)
      };

      return View(languageSelectorViewModel);
   }
}


