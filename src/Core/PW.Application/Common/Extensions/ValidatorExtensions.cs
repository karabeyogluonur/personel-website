using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace PW.Application.Common.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> ValidUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(url =>
                {
                    if (string.IsNullOrEmpty(url)) return true;

                    bool isUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                                 && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    return isUri;
                })
                .WithMessage("Please enter a valid URL (starting with http:// or https://).");
        }

        public static IRuleBuilderOptions<T, IFormFile?> MaxFileSize<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, int maxBytes)
        {
            long fileSizeInMb = maxBytes / 1024 / 1024;

            return ruleBuilder
                .Must(file => file is null || file.Length <= maxBytes)
                .WithMessage($"File size cannot exceed {fileSizeInMb}MB.");
        }

        public static IRuleBuilderOptions<T, IFormFile?> AllowedExtensions<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, params string[] extensions)
        {
            string[] validExtensions = extensions
                .Select(x => x.ToLowerInvariant().StartsWith(".") ? x : "." + x)
                .ToArray();

            string formattedExtensions = string.Join(", ", validExtensions);

            return ruleBuilder
                .Must(file =>
                {
                    if (file is null) return true;

                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return validExtensions.Contains(fileExtension);
                })
                .WithMessage($"Only {formattedExtensions} files are allowed.");
        }
    }
}
