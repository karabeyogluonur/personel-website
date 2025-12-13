using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using PW.Application.Interfaces.Localization;
using PW.Domain.Entities;

namespace PW.Web.Middlewares
{
    public class DynamicLocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CookieName = ".AspNetCore.Culture";

        public DynamicLocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILanguageService languageService)
        {
            IList<Language> publishedLanguages = await languageService.GetAllPublishedLanguagesAsync();
            if (publishedLanguages == null || !publishedLanguages.Any())
            {
                SetCulture(context, "en");
                await _next(context);
                return;
            }
            var defaultLanguage = publishedLanguages.FirstOrDefault(l => l.IsDefault) ?? publishedLanguages.First();
            string finalLanguageCode = string.Empty;

            string path = context.Request.Path.Value ?? string.Empty;
            string firstSegment = path.TrimStart('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

            Language matchedLanguage = publishedLanguages.FirstOrDefault(l =>
                l.Code.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

            if (matchedLanguage != null)
            {
                finalLanguageCode = matchedLanguage.Code;
            }
            else if (firstSegment.Length == 2 && Regex.IsMatch(firstSegment, "^[a-zA-Z]{2}$"))
            {
                context.Response.Redirect("/Error/404");
                return;
            }

            if (string.IsNullOrEmpty(finalLanguageCode))
            {
                if (context.Request.Cookies.TryGetValue(CookieName, out var cookieValue))
                {
                    var match = Regex.Match(cookieValue, @"uic=(?<lang>[a-zA-Z]{2})");
                    if (match.Success)
                    {
                        var cookieLang = match.Groups["lang"].Value;
                        if (publishedLanguages.Any(l => l.Code.Equals(cookieLang, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            finalLanguageCode = cookieLang;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(finalLanguageCode))
            {
                var userLanguages = context.Request.GetTypedHeaders().AcceptLanguage;
                if (userLanguages != null)
                {
                    var topLanguage = userLanguages.OrderByDescending(x => x.Quality ?? 1)
                                                   .FirstOrDefault();

                    if (topLanguage != null)
                    {
                        var isoCode = topLanguage.Value.ToString().Split('-')[0];
                        if (publishedLanguages.Any(l => l.Code.Equals(isoCode, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            finalLanguageCode = isoCode;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(finalLanguageCode))
            {
                finalLanguageCode = defaultLanguage.Code;
            }

            SetCulture(context, finalLanguageCode);
            await _next(context);
        }

        private void SetCulture(HttpContext context, string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            var requestCulture = new RequestCulture(culture);

            context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));
        }
    }
}
