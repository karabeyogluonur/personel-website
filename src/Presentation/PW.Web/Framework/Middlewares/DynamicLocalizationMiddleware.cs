using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;

namespace PW.Web.Framework.Middlewares;

public class DynamicLocalizationMiddleware
{
   private readonly RequestDelegate _next;
   private readonly string _cookieName = CookieRequestCultureProvider.DefaultCookieName;

   public DynamicLocalizationMiddleware(RequestDelegate next)
   {
      _next = next;
   }

   public async Task InvokeAsync(HttpContext context, ILanguageService languageService)
   {
      IList<LanguageLookupDto> publishedLanguages = await languageService.GetLanguagesLookupAsync();

      if (publishedLanguages == null || !publishedLanguages.Any())
      {
         SetCulture(context, "en");
         await _next(context);
         return;
      }

      LanguageLookupDto defaultLanguage = publishedLanguages.FirstOrDefault((LanguageLookupDto language) => language.IsDefault)
                                          ?? publishedLanguages.First();

      string finalLanguageCode = string.Empty;
      bool languageFoundInUrl = false;

      string path = context.Request.Path.Value ?? string.Empty;
      string firstSegment = path.TrimStart('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

      LanguageLookupDto? matchedLanguage = publishedLanguages.FirstOrDefault((LanguageLookupDto language) =>
          language.Code.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

      if (matchedLanguage != null)
      {
         finalLanguageCode = matchedLanguage.Code;
         languageFoundInUrl = true;
      }
      else if (firstSegment.Length == 2 && Regex.IsMatch(firstSegment, "^[a-zA-Z]{2}$"))
      {
         context.Response.Redirect("/Error/404");
         return;
      }

      string currentCookieLanguage = string.Empty;
      if (context.Request.Cookies.TryGetValue(_cookieName, out string? cookieValue) && cookieValue != null)
      {
         Match match = Regex.Match(cookieValue, @"uic=(?<lang>[a-zA-Z]{2})");

         if (match.Success)
            currentCookieLanguage = match.Groups["lang"].Value;
      }

      if (languageFoundInUrl)
      {
         if (!string.Equals(finalLanguageCode, currentCookieLanguage, StringComparison.InvariantCultureIgnoreCase))
            SetCookie(context, finalLanguageCode);
      }
      else
      {
         if (!string.IsNullOrEmpty(currentCookieLanguage) &&
             publishedLanguages.Any((LanguageLookupDto language) => language.Code.Equals(currentCookieLanguage, StringComparison.InvariantCultureIgnoreCase)))
            finalLanguageCode = currentCookieLanguage;
      }

      if (string.IsNullOrEmpty(finalLanguageCode))
      {
         IList<StringWithQualityHeaderValue> userLanguages = context.Request.GetTypedHeaders().AcceptLanguage;
         StringWithQualityHeaderValue? topLanguage = userLanguages?.OrderByDescending((StringWithQualityHeaderValue headerValue) => headerValue.Quality ?? 1).FirstOrDefault();

         if (topLanguage != null)
         {
            string isoCode = topLanguage.Value.ToString().Split('-')[0];
            if (publishedLanguages.Any((LanguageLookupDto language) => language.Code.Equals(isoCode, StringComparison.InvariantCultureIgnoreCase)))
            {
               finalLanguageCode = isoCode;
            }
         }
      }

      if (string.IsNullOrEmpty(finalLanguageCode))
         finalLanguageCode = defaultLanguage.Code;

      SetCulture(context, finalLanguageCode);

      await _next(context);
   }

   private void SetCulture(HttpContext context, string cultureCode)
   {
      CultureInfo culture = new CultureInfo(cultureCode);

      CultureInfo.CurrentCulture = culture;
      CultureInfo.CurrentUICulture = culture;

      RequestCulture requestCulture = new RequestCulture(culture);
      context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));
   }

   private void SetCookie(HttpContext context, string cultureCode)
   {
      CultureInfo culture = new CultureInfo(cultureCode);
      RequestCulture requestCulture = new RequestCulture(culture);

      string cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
      CookieOptions cookieOptions = new CookieOptions
      {
         Expires = DateTimeOffset.UtcNow.AddYears(1),
         IsEssential = true,
         Path = "/"
      };

      context.Response.Cookies.Append(_cookieName, cookieValue, cookieOptions);
   }
}
