using FluentValidation;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language.Validators
{
    public class LanguageCreateViewModelValidator : AbstractValidator<LanguageCreateViewModel>
    {
        public LanguageCreateViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Language name is required.")
                .MaximumLength(50).WithMessage("Language name cannot exceed 50 characters.");

            RuleFor(x => x.Code)
                 .NotEmpty().WithMessage("ISO Code is required.")
                 .Length(2).WithMessage("ISO Code must be exactly 2 characters (e.g., 'en', 'tr').")
                 .Matches("^[a-zA-Z]+$").WithMessage("ISO Code can only contain letters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

            RuleFor(x => x.FlagImage)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.FlagImage)
            .Must(HaveSupportedFileType)
            .When(x => x.FlagImage is not null)
            .WithMessage("Only .jpg, .jpeg, .png, and .svg file types are allowed.");

            RuleFor(x => x.FlagImage)
                .Must(HaveValidSize)
                .When(x => x.FlagImage is not null)
                .WithMessage("File size cannot exceed 2MB.");
        }

        private bool HaveSupportedFileType(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return true;

            const int maxFileSizeInBytes = 2 * 1024 * 1024; // 2 MB
            return file.Length <= maxFileSizeInBytes;
        }
    }
}
