using FluentValidation;
using PW.Web.Areas.Admin.Features.Language.ViewModels;
using PW.Application.Common.Extensions;

namespace PW.Web.Areas.Admin.Features.Language.Validators
{
    public class LanguageEditViewModelValidator : AbstractValidator<LanguageEditViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB

        public LanguageEditViewModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Language name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("ISO Code is required.")
                .Length(2).WithMessage("ISO Code must be exactly 2 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("ISO Code can only contain letters.");

            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);

            RuleFor(x => x.FlagImage)
                .AllowedExtensions(".jpg", ".jpeg", ".png", ".svg")
                .MaxFileSize(MaxFileSize);
        }
    }
}
