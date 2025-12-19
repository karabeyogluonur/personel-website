using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Category.ViewModels;

namespace PW.Web.Areas.Admin.Features.Category.Validators
{
    public class CategoryLocalizedViewModelValidator : AbstractValidator<CategoryLocalizedViewModel>
    {
        public CategoryLocalizedViewModelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ApplicationLimits.Category.NameMaxLength)
                .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Category.NameMaxLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(ApplicationLimits.Category.DescriptionMaxLength)
                .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Category.DescriptionMaxLength} characters.");

            RuleFor(x => x.CoverImage)
                .AllowedExtensions(ApplicationLimits.Category.AllowedCoverImageExtensions)
                .MaxFileSize(ApplicationLimits.Category.MaxCoverImageSizeBytes);
        }
    }
}
