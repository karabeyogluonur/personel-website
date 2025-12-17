using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Validators
{
    public class TechnologyEditViewModelValidator : AbstractValidator<TechnologyEditViewModel>
    {
        public TechnologyEditViewModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Technology ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
                .WithMessage($"Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
                .WithMessage($"Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");

            RuleFor(x => x.DocumentationUrl)
                .NotEmpty().WithMessage("Documentation url is required.")
                .MaximumLength(ApplicationLimits.Technology.UrlMaxLength)
                .WithMessage($"URL cannot exceed {ApplicationLimits.Technology.UrlMaxLength} characters.")
                .ValidUrl();

            RuleFor(x => x.IconImage)
                .AllowedExtensions(ApplicationLimits.Technology.AllowedIconExtensions)
                .MaxFileSize(ApplicationLimits.Technology.MaxIconSizeBytes);

            RuleForEach(x => x.Locales).SetValidator(new TechnologyLocalizedViewModelValidator());
        }
    }
}
