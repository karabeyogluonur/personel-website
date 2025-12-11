using FluentValidation;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Language.Validators
{
    public class LanguageEditViewModelValidator : AbstractValidator<LanguageEditViewModel>
    {
        public LanguageEditViewModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Language name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("ISO Code is required.")
                .Length(2).WithMessage("ISO Code must be exactly 2 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("ISO Code can only contain letters.");

            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);

            RuleFor(x => x.FlagImage)
            .Must(HaveSupportedFileType)
            .When(x => x.FlagImage is not null)
            .WithMessage("Only .jpg, .jpeg, .png, and .svg file types are allowed.");

            RuleFor(x => x.FlagImage)
                .Must(HaveValidSize)
                .When(x => x.FlagImage is not null)
                .WithMessage("File size cannot exceed 2MB.");
        }

        private bool HaveSupportedFileType(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return new[] { ".jpg", ".jpeg", ".png", ".svg" }.Contains(ext);
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return true;
            return file.Length <= 2 * 1024 * 1024;
        }
    }
}
