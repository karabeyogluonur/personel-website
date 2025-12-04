using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Interfaces.Localization;
using PW.Services.Localization;

namespace PW.Services
{
    public static class DependencyInjection
    {
        public static void AddServiceServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILanguageService, LanguageService>();
        }
    }
}
