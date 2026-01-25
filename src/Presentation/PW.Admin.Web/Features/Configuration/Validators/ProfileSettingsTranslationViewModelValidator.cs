using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Features.Configuration.Validators;

public class ProfileSettingsTranslationViewModelValidator : AbstractValidator<ProfileSettingsTranslationViewModel>
{
    public ProfileSettingsTranslationViewModelValidator()
    {
        RuleFor(translationViewModel => translationViewModel.FirstName)
            .MaximumLength(ApplicationLimits.ProfileSettings.FirstNameMaxLength)
            .WithMessage($"First Name cannot exceed {ApplicationLimits.ProfileSettings.FirstNameMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.LastName)
            .MaximumLength(ApplicationLimits.ProfileSettings.LastNameMaxLength)
            .WithMessage($"Last Name cannot exceed {ApplicationLimits.ProfileSettings.LastNameMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.JobTitle)
            .MaximumLength(ApplicationLimits.ProfileSettings.JobTitleMaxLength)
            .WithMessage($"Job Title cannot exceed {ApplicationLimits.ProfileSettings.JobTitleMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.Biography)
            .MaximumLength(ApplicationLimits.ProfileSettings.BiographyMaxLength)
            .WithMessage($"Biography cannot exceed {ApplicationLimits.ProfileSettings.BiographyMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.AvatarImage)
            .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
            .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);

        RuleFor(translationViewModel => translationViewModel.CoverImage)
            .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
            .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);
    }
}
