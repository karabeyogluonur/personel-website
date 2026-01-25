using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Admin.Web.Features.Configuration.ViewModels;

namespace PW.Admin.Web.Features.Configuration.Validators;

public class GeneralSettingsTranslationViewModelValidator : AbstractValidator<GeneralSettingsTranslationViewModel>
{
    public GeneralSettingsTranslationViewModelValidator()
    {
        RuleFor(translationViewModel => translationViewModel.SiteTitle)
            .MaximumLength(ApplicationLimits.GeneralSettings.SiteTitleMaxLength)
            .WithMessage($"Site Title cannot exceed {ApplicationLimits.GeneralSettings.SiteTitleMaxLength} characters.");

        RuleFor(translationViewModel => translationViewModel.LightThemeLogoImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(translationViewModel => translationViewModel.DarkThemeLogoImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(translationViewModel => translationViewModel.LightThemeFaviconImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

        RuleFor(translationViewModel => translationViewModel.DarkThemeFaviconImage)
            .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
            .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);
    }
}
