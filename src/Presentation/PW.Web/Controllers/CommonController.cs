using Microsoft.AspNetCore.Mvc;

using PW.Application.Common.Extensions;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;
using PW.Application.Models.Dtos.Localization;

namespace PW.Web.Controllers;

public class CommonController : BasePublicController
{
   private readonly ILanguageService _languageService;
   private readonly IWorkContext _workContext;

   public CommonController(ILanguageService languageService, IWorkContext workContext)
   {
      _languageService = languageService;
      _workContext = workContext;
   }

   public async Task<IActionResult> ChangeLanguage(string culture, string returnUrl)
   {
      LanguageDetailDto? language = await _languageService.GetLanguageByCodeAsync(culture);

      if (language == null)
         return Redirect("/");

      await _workContext.SetCurrentLanguageAsync(language);

      string newUrl = returnUrl.SwitchLanguageInUrl(language.Code);

      return Redirect(newUrl);
   }
}
