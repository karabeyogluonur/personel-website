using System.Text.RegularExpressions;

namespace PW.Application.Common.Extensions
{
    public static class UrlExtensions
    {
        public static string SwitchLanguageInUrl(this string url, string newCulture)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "/")
                return $"/{newCulture}";

            string queryString = string.Empty;
            int queryIndex = url.IndexOf('?');

            string path = url;
            if (queryIndex >= 0)
            {
                queryString = url.Substring(queryIndex);
                path = url.Substring(0, queryIndex);
            }

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            bool hasLanguagePrefix = segments.Count > 0 &&
                                     Regex.IsMatch(segments[0], "^[a-zA-Z]{2}$");

            if (hasLanguagePrefix)
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
