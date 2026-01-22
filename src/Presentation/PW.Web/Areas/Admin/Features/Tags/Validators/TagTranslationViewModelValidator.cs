using FluentValidation;
using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.Tags.ViewModels;

namespace PW.Web.Areas.Admin.Features.Tags.Validators;

public class TagTranslationViewModelValidator : AbstractValidator<TagTranslationViewModel>
{
    public TagTranslationViewModelValidator()
    {
        RuleFor(translationViewModel => translationViewModel.Name)
            .MaximumLength(ApplicationLimits.Tag.NameMaxLength)
            .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Tag.NameMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.Description)
            .MaximumLength(ApplicationLimits.Tag.DescriptionMaxLength)
            .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Tag.DescriptionMaxLength} characters.");
    }
}
