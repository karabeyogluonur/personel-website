using FluentValidation;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Technology.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.Validators
{
    public class TechnologyEditViewModelValidator : AbstractValidator<TechnologyEditViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB
        public TechnologyEditViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description).MaximumLength(1000);

            RuleFor(x => x.DocumentationUrl).ValidUrl();

            RuleFor(x => x.IconImage)
                .MaxFileSize(MaxFileSize)
                .AllowedExtensions(".png", ".jpg", ".jpeg", ".svg")
                .When(x => x.IconImage is null);

            RuleForEach(x => x.Locales).SetValidator(new TechnologyLocalizedViewModelValidator());
        }
    }
}
