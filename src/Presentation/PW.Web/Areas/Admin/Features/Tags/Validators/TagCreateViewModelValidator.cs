using FluentValidation;
using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.Tags.ViewModels;

namespace PW.Web.Areas.Admin.Features.Tags.Validators;

public class TagCreateViewModelValidator : AbstractValidator<TagCreateViewModel>
{
    public TagCreateViewModelValidator()
    {
        RuleFor(createViewModel => createViewModel.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(ApplicationLimits.Tag.NameMaxLength)
            .WithMessage($"Name cannot exceed {ApplicationLimits.Tag.NameMaxLength} characters.");

        RuleFor(createViewModel => createViewModel.Description)
            .MaximumLength(ApplicationLimits.Tag.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {ApplicationLimits.Tag.DescriptionMaxLength} characters.");

        RuleFor(editViewModel => editViewModel.ColorHex)
        .MaximumLength(ApplicationLimits.Tag.ColorHexMaxLength)
        .WithMessage($"Badge color hex cannot exceed {ApplicationLimits.Tag.ColorHexMaxLength} characters.");

        RuleForEach(createViewModel => createViewModel.Translations)
            .SetValidator(new TagTranslationViewModelValidator());
    }
}
