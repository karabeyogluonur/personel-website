using FluentValidation;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Validators
{
    public class TechnologyLocalizedViewModelValidator : AbstractValidator<TechnologyLocalizedViewModel>
    {
        public TechnologyLocalizedViewModelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Localized Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Localized Description cannot exceed 1000 characters.");
        }
    }
}
