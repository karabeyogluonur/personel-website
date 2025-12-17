using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language.Validators
{
    public class LanguageEditViewModelValidator : AbstractValidator<LanguageEditViewModel>
    {
        public LanguageEditViewModelValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid language ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Language name is required.")
                .MaximumLength(ApplicationLimits.Common.NameMaxLength)
                .WithMessage($"Language name cannot exceed {ApplicationLimits.Common.NameMaxLength} characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("ISO Code is required.")
                .Length(ApplicationLimits.Language.CodeMaxLength)
                .WithMessage($"ISO Code must be exactly {ApplicationLimits.Language.CodeMaxLength} characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("ISO Code can only contain letters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

            RuleFor(x => x.FlagImage)
                .AllowedExtensions(ApplicationLimits.Language.AllowedFlagExtensions)
                .MaxFileSize(ApplicationLimits.Language.MaxFlagSizeBytes);
        }
    }
}
