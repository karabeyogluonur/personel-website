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
            var cultureCode = requestCulture?.UICulture.Name ?? CultureInfo.CurrentUICulture.Name;
            var isoCode = requestCulture?.UICulture.TwoLetterISOLanguageName ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var language = await _languageService.GetLanguageByCodeAsync(isoCode);

            if (language == null || !language.IsPublished)
                language = (await _languageService.GetAllPublishedLanguagesAsync()).FirstOrDefault(x => x.IsDefault);

            _cachedLanguage = language;
            return _cachedLanguage;
        }
    }
}
