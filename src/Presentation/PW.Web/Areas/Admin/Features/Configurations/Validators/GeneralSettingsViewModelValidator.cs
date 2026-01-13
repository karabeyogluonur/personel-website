using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Configurations.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configurations.Validators;

public class GeneralSettingsViewModelValidator : AbstractValidator<GeneralSettingsViewModel>
{
    public GeneralSettingsViewModelValidator()
    {
        RuleFor(settingsViewModel => settingsViewModel.SiteTitle)
            .NotEmpty().WithMessage("Site Title is required.")
            .MaximumLength(ApplicationLimits.GeneralSettings.SiteTitleMaxLength)
            .WithMessage($"Site Title cannot exceed {ApplicationLimits.GeneralSettings.SiteTitleMaxLength} characters.");

        RuleFor(settingsViewModel => settingsViewModel.LightThemeLogoImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(settingsViewModel => settingsViewModel.DarkThemeLogoImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(settingsViewModel => settingsViewModel.LightThemeFaviconImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(settingsViewModel => settingsViewModel.DarkThemeFaviconImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleForEach(settingsViewModel => settingsViewModel.Translations)
            .SetValidator(new GeneralSettingsTranslationViewModelValidator());
    }
}
