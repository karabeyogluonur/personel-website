using PW.Application.Common.Constants;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;

namespace PW.Web.Middlewares
{
    public class CultureRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IWorkContext workContext, ILanguageService languageService)
        {
            string path = context.Request.Path.Value ?? string.Empty;

            if (path.StartsWith("/api") || path.StartsWith("/uploads") || Path.HasExtension(path) || path.StartsWith(AreaNames.Admin))
            {
                await _next(context);
                return;
            }

            var publishedLanguages = await languageService.GetAllPublishedLanguagesAsync();
            string firstSegment = path.TrimStart('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

            bool hasCultureInUrl = publishedLanguages.Any(l => l.Code.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

            if (!hasCultureInUrl)
            {
                var currentLang = await workContext.GetCurrentLanguageAsync();
                var cultureCode = currentLang.Code;
                var newPath = $"/{cultureCode}{path}";

                context.Response.Redirect(newPath, permanent: false);
                return;
            }

            await _next(context);
        }
    }
}
