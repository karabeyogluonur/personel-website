using FluentValidation;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;
using PW.Application.Common.Extensions;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators
{
    public class ProfileSettingsViewModelValidator : AbstractValidator<ProfileSettingsViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB

        public ProfileSettingsViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

            RuleFor(x => x.JobTitle).MaximumLength(100).WithMessage("Job Title cannot exceed 100 characters.");
            RuleFor(x => x.Biography).MaximumLength(1000).WithMessage("Biography cannot exceed 1000 characters.");

            RuleFor(x => x.AvatarImage)
                .AllowedExtensions(".jpg", ".jpeg", ".png")
                .MaxFileSize(MaxFileSize);

            RuleFor(x => x.CoverImage)
                .AllowedExtensions(".jpg", ".jpeg", ".png")
                .MaxFileSize(MaxFileSize);

            RuleForEach(x => x.Locales).SetValidator(new ProfileSettingsLocalizedModelValidator());
        }
    }

}
