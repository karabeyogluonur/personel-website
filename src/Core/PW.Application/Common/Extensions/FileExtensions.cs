using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;

namespace PW.Application.Common.Extensions
{
    public static class FileExtensions
    {
        public static string GetExtension(this IFormFile file)
        {
            return Path.GetExtension(file.FileName).ToLowerInvariant();
        }

        public static string ToUrlSlug(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "";

            string str = phrase.ToLowerInvariant();

            str = str.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                     .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", "-").Trim();

            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}
