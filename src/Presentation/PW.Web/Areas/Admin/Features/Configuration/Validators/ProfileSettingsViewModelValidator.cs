using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators;

public class ProfileSettingsViewModelValidator : AbstractValidator<ProfileSettingsViewModel>
{
   public ProfileSettingsViewModelValidator()
   {
      RuleFor(settingsViewModel => settingsViewModel.FirstName)
          .NotEmpty().WithMessage("First Name is required.")
          .MaximumLength(ApplicationLimits.ProfileSettings.FirstNameMaxLength)
          .WithMessage($"First Name cannot exceed {ApplicationLimits.ProfileSettings.FirstNameMaxLength} characters.");

      RuleFor(settingsViewModel => settingsViewModel.LastName)
          .NotEmpty().WithMessage("Last Name is required.")
          .MaximumLength(ApplicationLimits.ProfileSettings.LastNameMaxLength)
          .WithMessage($"Last Name cannot exceed {ApplicationLimits.ProfileSettings.LastNameMaxLength} characters.");

      RuleFor(settingsViewModel => settingsViewModel.JobTitle)
          .MaximumLength(ApplicationLimits.ProfileSettings.JobTitleMaxLength)
          .WithMessage($"Job Title cannot exceed {ApplicationLimits.ProfileSettings.JobTitleMaxLength} characters.");

      RuleFor(settingsViewModel => settingsViewModel.Biography)
          .MaximumLength(ApplicationLimits.ProfileSettings.BiographyMaxLength)
          .WithMessage($"Biography cannot exceed {ApplicationLimits.ProfileSettings.BiographyMaxLength} characters.");

      RuleFor(settingsViewModel => settingsViewModel.AvatarImage)
          .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
          .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);

      RuleFor(settingsViewModel => settingsViewModel.CoverImage)
          .AllowedExtensions(ApplicationLimits.ProfileSettings.AllowedImageExtensions)
          .MaxFileSize(ApplicationLimits.ProfileSettings.MaxImageSizeBytes);

      RuleForEach(settingsViewModel => settingsViewModel.Translations)
          .SetValidator(new ProfileSettingsTranslationViewModelValidator());
   }
}
