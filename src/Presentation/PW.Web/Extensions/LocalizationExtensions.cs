using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using PW.Application.Interfaces.Localization;

namespace PW.Web.Extensions
{
    public record LocalizationConfigResult(string DefaultCulture, IRouteConstraint CultureConstraint);

    public static class LocalizationExtensions
    {
        public static LocalizationConfigResult AddDatabaseLocalization(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var languageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();
            var publishedLanguages = languageService.GetAllPublishedLanguages();

            var defaultLanguage = publishedLanguages.FirstOrDefault(l => l.IsDefault) ?? publishedLanguages.FirstOrDefault();
            var supportedCultures = publishedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();

            var cultureRegex = string.Join("|", publishedLanguages.Select(l => Regex.Escape(l.Code)));
            var cultureConstraint = new RegexRouteConstraint($"^({cultureRegex})$");
            var defaultCultureCode = defaultLanguage?.Code ?? "en";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
                {
                    RouteDataStringKey = "culture",
                    UIRouteDataStringKey = "culture",
                    Options = options
                });
                options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
                options.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
                options.RequestCultureProviders.Insert(3, new QueryStringRequestCultureProvider());
            });
            ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.CurrentCulture;
            return new LocalizationConfigResult(defaultCultureCode, cultureConstraint);
        }
    }
}
