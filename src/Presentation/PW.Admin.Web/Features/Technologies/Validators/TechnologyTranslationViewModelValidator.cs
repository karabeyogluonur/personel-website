using FluentValidation;

using PW.Application.Common.Constants;
using PW.Admin.Web.Features.Technologies.ViewModels;

namespace PW.Admin.Web.Features.Technologies.Validators;

public class TechnologyTranslationViewModelValidator : AbstractValidator<TechnologyTranslationViewModel>
{
    public TechnologyTranslationViewModelValidator()
    {
        RuleFor(translationViewModel => translationViewModel.Name)
            .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
            .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.Description)
            .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
            .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");
    }
}
