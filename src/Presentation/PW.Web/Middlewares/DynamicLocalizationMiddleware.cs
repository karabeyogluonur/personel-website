using System.Globalization;
using System.Text.RegularExpressions; // Regex için gerekli
using Microsoft.AspNetCore.Localization;
using PW.Application.Interfaces.Localization;
using PW.Domain.Entities;

namespace PW.Web.Middlewares
{
    public class DynamicLocalizationMiddleware
    {
        private readonly RequestDelegate _next;

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

            var defaultLanguage = publishedLanguages.FirstOrDefault(language => language.IsDefault) ?? publishedLanguages.First();

            string path = context.Request.Path.Value;
            string firstSegment = string.Empty;

            if (!string.IsNullOrEmpty(path))
            {
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    firstSegment = segments[0];
                }
            }

            Language matchedLanguage = publishedLanguages.FirstOrDefault(l => l.Code.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

            if (matchedLanguage != null)
                SetCulture(context, matchedLanguage.Code);

            else
            {
                if (firstSegment.Length == 2 && Regex.IsMatch(firstSegment, "^[a-zA-Z]{2}$"))
                {
                    context.Response.Redirect("/Error/404");
                    return;
                }

                SetCulture(context, defaultLanguage.Code);
            }

            await _next(context);
        }

        private void SetCulture(HttpContext context, string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var requestCulture = new RequestCulture(culture);
            context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));
        }
    }
}
