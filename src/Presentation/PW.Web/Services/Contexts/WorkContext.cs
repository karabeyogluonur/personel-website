using System.Globalization;

using Microsoft.AspNetCore.Localization;
using PW.Application.Common.Interfaces;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;

namespace PW.Web.Services.Contexts;

public class WorkContext : IWorkContext
{
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly ILanguageService _languageService;
   private LanguageDetailDto _cachedLanguage;

   public WorkContext(IHttpContextAccessor httpContextAccessor, ILanguageService languageService)
   {
      _httpContextAccessor = httpContextAccessor;
      _languageService = languageService;
   }

   public async Task<LanguageDetailDto> GetCurrentLanguageAsync()
   {
      if (_cachedLanguage != null)
         return _cachedLanguage;

      IRequestCultureFeature? requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();

      string? isoCode = requestCultureFeature?.RequestCulture.UICulture.TwoLetterISOLanguageName;

      if (string.IsNullOrEmpty(isoCode))
         isoCode = "en";

      LanguageDetailDto? language = await _languageService.GetLanguageByCodeAsync(isoCode);

      if (language == null || !language.IsPublished)
      {
         language = await _languageService.GetDefaultLanguageAsync();

         if (language == null)
            throw new InvalidOperationException("Default language not found.");
      }

      _cachedLanguage = language;
      return _cachedLanguage;
   }

   public async Task SetCurrentLanguageAsync(LanguageDetailDto languageDetailDto)
   {
      if (languageDetailDto == null) return;

      _cachedLanguage = languageDetailDto;

      CultureInfo? cultureInfo = new CultureInfo(languageDetailDto.Code);

      CultureInfo.CurrentCulture = cultureInfo;
      CultureInfo.CurrentUICulture = cultureInfo;

      RequestCulture requestCulture = new RequestCulture(cultureInfo);
      _httpContextAccessor.HttpContext?.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));

      string cookieName = CookieRequestCultureProvider.DefaultCookieName;
      string cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

      CookieOptions cookieOptions = new CookieOptions
      {
         Expires = DateTimeOffset.UtcNow.AddYears(1),
         IsEssential = true,
         Path = "/",
         HttpOnly = false,
         Secure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false
      };

      _httpContextAccessor.HttpContext?.Response.Cookies.Append(
          cookieName,
          cookieValue,
          cookieOptions
      );

      await Task.CompletedTask;
   }
}
