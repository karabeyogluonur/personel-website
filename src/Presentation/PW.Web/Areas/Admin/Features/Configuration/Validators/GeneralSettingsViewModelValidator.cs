using FluentValidation;
using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators
{
    public class GeneralSettingsViewModelValidator : AbstractValidator<GeneralSettingsViewModel>
    {
        public GeneralSettingsViewModelValidator()
        {
            RuleFor(x => x.SiteTitle)
                .NotEmpty().WithMessage("Site Title is required.")
                .MaximumLength(ApplicationLimits.GeneralSettings.SiteTitleMaxLength)
                .WithMessage($"Site Title cannot exceed {ApplicationLimits.GeneralSettings.SiteTitleMaxLength} characters.");

            RuleFor(x => x.LightThemeLogoImage)
                .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
                .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

            RuleFor(x => x.DarkThemeLogoImage)
                .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedLogoExtensions)
                .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

            RuleFor(x => x.LightThemeFaviconImage)
                .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
                .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

            RuleFor(x => x.DarkThemeFaviconImage)
                .AllowedExtensions(ApplicationLimits.GeneralSettings.AllowedFaviconExtensions)
                .MaxFileSize(ApplicationLimits.GeneralSettings.MaxFileSizeBytes);

            RuleForEach(x => x.Locales).SetValidator(new GeneralSettingsLocalizedModelValidator());
        }
    }
}
