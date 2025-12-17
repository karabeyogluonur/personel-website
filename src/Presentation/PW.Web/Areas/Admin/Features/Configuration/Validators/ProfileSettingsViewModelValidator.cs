using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators
{
    public class ProfileSettingsViewModelValidator : AbstractValidator<ProfileSettingsViewModel>
    {
        public ProfileSettingsViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(ApplicationLimits.ProfileSettings.FirstNameMaxLength)
                .WithMessage($"First Name cannot exceed {ApplicationLimits.ProfileSettings.FirstNameMaxLength} characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(ApplicationLimits.ProfileSettings.LastNameMaxLength)
                .WithMessage($"Last Name cannot exceed {ApplicationLimits.ProfileSettings.LastNameMaxLength} characters.");

            RuleFor(x => x.JobTitle)
                .MaximumLength(ApplicationLimits.ProfileSettings.JobTitleMaxLength)
                .WithMessage($"Job Title cannot exceed {ApplicationLimits.ProfileSettings.JobTitleMaxLength} characters.");

            RuleFor(x => x.Biography)
                .MaximumLength(ApplicationLimits.ProfileSettings.BiographyMaxLength)
                .WithMessage($"Biography cannot exceed {ApplicationLimits.ProfileSettings.BiographyMaxLength} characters.");

            RuleFor(x => x.AvatarImage)
                .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
                .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);

            RuleFor(x => x.CoverImage)
                .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
                .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);

            RuleForEach(x => x.Locales).SetValidator(new ProfileSettingsLocalizedModelValidator());
        }
    }
}
