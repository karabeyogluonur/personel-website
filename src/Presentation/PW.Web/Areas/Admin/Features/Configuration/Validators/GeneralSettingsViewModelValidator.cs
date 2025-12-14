using FluentValidation;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators
{
    public class GeneralSettingsViewModelValidator : AbstractValidator<GeneralSettingsViewModel>
    {
        private const int MaxFileSize = 2 * 1024 * 1024; //2MB

        public GeneralSettingsViewModelValidator()
        {
            RuleFor(x => x.SiteTitle)
                .NotEmpty().WithMessage("Site Title is required.")
                .MaximumLength(256).WithMessage("Site Title cannot exceed 256 characters.");

            RuleFor(x => x.LightThemeLogoImage)
                .AllowedExtensions(".jpg", ".jpeg", ".png")
                .MaxFileSize(MaxFileSize);

            RuleFor(x => x.DarkThemeLogoImage)
                .AllowedExtensions(".jpg", ".jpeg", ".png")
                .MaxFileSize(MaxFileSize);

            RuleFor(x => x.LightThemeFaviconImage)
                .AllowedExtensions(".ico", ".png")
                .MaxFileSize(MaxFileSize);

            RuleFor(x => x.DarkThemeFaviconImage)
                .AllowedExtensions(".ico", ".png")
                .MaxFileSize(MaxFileSize);

            RuleForEach(x => x.Locales).SetValidator(new GeneralSettingsLocalizedModelValidator());
        }
    }
}
