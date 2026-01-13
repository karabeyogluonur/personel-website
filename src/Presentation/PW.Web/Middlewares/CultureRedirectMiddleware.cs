using PW.Application.Common.Constants;
using PW.Application.Common.Interfaces;
using PW.Application.Interfaces.Localization;

namespace PW.Web.Middlewares;

public class CultureRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public CultureRedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IWorkContext workContext, ILanguageService languageService)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith($"/{AreaNames.Admin}", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/auth/logout", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/error", StringComparison.OrdinalIgnoreCase) ||
            Path.HasExtension(path))
        {
            await _next(context);
            return;
        }

        var publishedLanguages = await languageService.GetLanguagesLookupAsync();
        var validLanguageCodes = publishedLanguages.Select(x => x.Code).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        string firstSegment = segments.FirstOrDefault() ?? string.Empty;

        bool hasValidCulture = validLanguageCodes.Contains(firstSegment);

        if (!hasValidCulture)
        {
            var currentLang = await workContext.GetCurrentLanguageAsync();

            var request = context.Request;
            var query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;

            var newPath = $"/{currentLang.Code}{path}{query}";

            context.Response.Redirect(newPath, permanent: false);
            return;
        }

        await _next(context);
    }
}
