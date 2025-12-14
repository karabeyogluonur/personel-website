using FluentValidation;
using PW.Web.Areas.Admin.Features.Language.ViewModels;
using PW.Application.Common.Extensions;

namespace PW.Web.Areas.Admin.Features.Language.Validators
{
    public class LanguageCreateViewModelValidator : AbstractValidator<LanguageCreateViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB

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
                .NotNull().WithMessage("Flag image is required.")
                .AllowedExtensions(".jpg", ".jpeg", ".png", ".svg")
                .MaxFileSize(MaxFileSize);
        }
    }
}
