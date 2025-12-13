using FluentValidation;
using PW.Web.Areas.Admin.Features.Configuration.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.Validators
{
    public class ProfileSettingsLocalizedModelValidator : AbstractValidator<ProfileSettingsLocalizedViewModel>
    {
        public ProfileSettingsLocalizedModelValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

            RuleFor(x => x.JobTitle)
                .MaximumLength(100).WithMessage("Job Title cannot exceed 100 characters.");

            RuleFor(x => x.Biography)
                .MaximumLength(1000).WithMessage("Biography cannot exceed 1000 characters.");

            RuleFor(x => x.AvatarImage)
                .Must(HaveSupportedFileType).When(x => x.AvatarImage != null)
                .WithMessage("Only .jpg, .jpeg, and .png files are allowed for Localized Avatar.")
                .Must(HaveValidSize).When(x => x.AvatarImage != null)
                .WithMessage("Localized Avatar file size cannot exceed 2MB.");

            RuleFor(x => x.CoverImage)
                .Must(HaveSupportedFileType).When(x => x.CoverImage != null)
                .WithMessage("Only .jpg, .jpeg, and .png files are allowed for Localized Cover Image.")
                .Must(HaveValidSize).When(x => x.CoverImage != null)
                .WithMessage("Localized Cover Image file size cannot exceed 2MB.");
        }

        private bool HaveSupportedFileType(IFormFile file)
        {
            if (file == null) return true;
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return new[] { ".jpg", ".jpeg", ".png" }.Contains(ext);
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return true;
            return file.Length <= 2 * 1024 * 1024;
        }
    }

}
