using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Admin.Web.Features.Technologies.ViewModels;

namespace PW.Admin.Web.Features.Technologies.Validators;

public class TechnologyCreateViewModelValidator : AbstractValidator<TechnologyCreateViewModel>
{
    public TechnologyCreateViewModelValidator()
    {
        RuleFor(createViewModel => createViewModel.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
            .WithMessage($"Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

        RuleFor(createViewModel => createViewModel.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");

        RuleFor(createViewModel => createViewModel.IconImage)
            .NotNull().WithMessage("Icon image is required.")
            .AllowedExtensions(ApplicationLimits.Technology.AllowedIconExtensions)
            .MaxFileSize(ApplicationLimits.Technology.MaxIconSizeBytes);

        RuleForEach(createViewModel => createViewModel.Translations)
            .SetValidator(new TechnologyTranslationViewModelValidator());
    }
}
