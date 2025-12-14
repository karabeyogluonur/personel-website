using System.Globalization;
using Microsoft.AspNetCore.Localization;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;
using PW.Domain.Entities;

namespace PW.Web
{
    public class WorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private Language _cachedLanguage;

        public WorkContext(
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
        }

        public async Task<Language> GetCurrentLanguageAsync()
        {
            if (_cachedLanguage != null)
                return _cachedLanguage;

            var requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();

            string isoCode = requestCultureFeature?.RequestCulture.UICulture.TwoLetterISOLanguageName;

            if (string.IsNullOrEmpty(isoCode))
            {
                isoCode = "en";
            }

            var language = await _languageService.GetLanguageByCodeAsync(isoCode);

            if (language == null || !language.IsPublished)
            {
                language = await _languageService.GetDefaultLanguageAsync();

                if (language == null)
                    throw new InvalidOperationException("Default language not found.");
            }

            _cachedLanguage = language;
            return _cachedLanguage;
        }

        public async Task SetCurrentLanguageAsync(Language language)
        {
            if (language == null) return;

            _cachedLanguage = language;

            var cultureInfo = new CultureInfo(language.Code);

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            var requestCulture = new RequestCulture(cultureInfo);
            _httpContextAccessor.HttpContext?.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));

            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            var cookieOptions = new CookieOptions
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
}
