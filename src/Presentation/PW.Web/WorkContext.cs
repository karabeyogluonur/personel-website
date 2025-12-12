using System.Globalization;
using Microsoft.AspNetCore.Localization;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;
using PW.Domain.Entities;

namespace PW.Web.Services
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

            var requestCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            var isoCode = requestCulture?.UICulture.TwoLetterISOLanguageName ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var language = await _languageService.GetLanguageByCodeAsync(isoCode);

            if (language == null || !language.IsPublished)
                language = (await _languageService.GetAllPublishedLanguagesAsync()).FirstOrDefault(x => x.IsDefault);

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
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var requestCulture = new RequestCulture(cultureInfo);
            _httpContextAccessor.HttpContext?.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            var cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                cookieOptions
            );

            await Task.CompletedTask;
        }
    }
}
