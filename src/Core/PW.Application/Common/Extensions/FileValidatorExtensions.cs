using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace PW.Application.Common.Extensions
{
    public static class FileValidatorExtensions
    {
        public static IRuleBuilderOptions<T, IFormFile> MaxFileSize<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, int maxBytes)
        {
            return ruleBuilder
                .Must(file => file == null || file.Length <= maxBytes)
                .WithMessage($"File size cannot exceed {maxBytes / 1024 / 1024}MB.");
        }

        public static IRuleBuilderOptions<T, IFormFile> AllowedExtensions<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, params string[] extensions)
        {
            var validExtensions = extensions.Select(e => e.ToLowerInvariant().StartsWith(".") ? e : "." + e).ToArray();
            var formattedExtensions = string.Join(", ", validExtensions);

            return ruleBuilder
                .Must(file =>
                {
                    if (file == null) return true;
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return validExtensions.Contains(ext);
                })
                .WithMessage($"Only {formattedExtensions} files are allowed.");
        }
    }
}
