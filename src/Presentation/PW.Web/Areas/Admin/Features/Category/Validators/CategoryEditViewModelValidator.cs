using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Category.ViewModels;

namespace PW.Web.Areas.Admin.Features.Category.Validators
{
    public class CategoryEditViewModelValidator : AbstractValidator<CategoryEditViewModel>
    {
        public CategoryEditViewModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Category ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(ApplicationLimits.Category.NameMaxLength)
                .WithMessage($"Name cannot exceed {ApplicationLimits.Category.NameMaxLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(ApplicationLimits.Category.DescriptionMaxLength)
                .WithMessage($"Description cannot exceed {ApplicationLimits.Category.DescriptionMaxLength} characters.");

            RuleFor(x => x.CoverImage)
                .AllowedExtensions(ApplicationLimits.Category.AllowedCoverImageExtensions)
                .MaxFileSize(ApplicationLimits.Category.MaxCoverImageSizeBytes);

            RuleForEach(x => x.Locales).SetValidator(new CategoryLocalizedViewModelValidator());
        }
    }
}
