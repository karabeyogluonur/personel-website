using FluentValidation;
using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Validators
{
    public class TechnologyLocalizedViewModelValidator : AbstractValidator<TechnologyLocalizedViewModel>
    {
        public TechnologyLocalizedViewModelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
                .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
                .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");
        }
    }
}
