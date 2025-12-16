using FluentValidation;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Validators
{
    public class TechnologyCreateViewModelValidator : AbstractValidator<TechnologyCreateViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB
        public TechnologyCreateViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.DocumentationUrl)
                .NotEmpty().WithMessage("Documentation url is required.")
                .ValidUrl().WithMessage("Please enter a valid URL (e.g. https://docs.microsoft.com).");

            RuleFor(x => x.IconImage)
                .NotNull().WithMessage("Icon image is required.")
                .MaxFileSize(MaxFileSize)
                .AllowedExtensions(".png", ".jpg", ".jpeg", ".svg");

            RuleForEach(x => x.Locales).SetValidator(new TechnologyLocalizedViewModelValidator());
        }
    }


}
