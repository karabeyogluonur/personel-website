using System.Text.RegularExpressions;

namespace PW.Application.Common.Extensions
{
    public static class UrlExtensions
    {
        public static string SwitchLanguageInUrl(this string url, string newCulture)
        {
            if (string.IsNullOrWhiteSpace(url)) return "/";
            if (url == "/") return $"/{newCulture}";

            string path = url;
            string queryString = string.Empty;
            int queryIndex = url.IndexOf('?');

            if (queryIndex >= 0)
            {
                path = url.Substring(0, queryIndex);
                queryString = url.Substring(queryIndex);
            }

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (segments.Count == 0)
            {
                return $"/{newCulture}{queryString}";
            }

            bool firstSegmentIsLanguage = Regex.IsMatch(segments[0], "^[a-zA-Z]{2}$");

            if (firstSegmentIsLanguage)
            {
                segments[0] = newCulture;
            }
            else
            {
                segments.Insert(0, newCulture);
            }

            return "/" + string.Join("/", segments) + queryString;
        }
    }
}
