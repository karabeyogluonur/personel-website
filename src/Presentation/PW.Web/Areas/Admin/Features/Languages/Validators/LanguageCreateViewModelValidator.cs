using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Languages.ViewModels;

namespace PW.Web.Areas.Admin.Features.Languages.Validators;

public class LanguageCreateViewModelValidator : AbstractValidator<LanguageCreateViewModel>
{
   public LanguageCreateViewModelValidator()
   {
      RuleFor(createViewModel => createViewModel.Name)
          .NotEmpty().WithMessage("Language name is required.")
          .MaximumLength(ApplicationLimits.Common.NameMaxLength)
          .WithMessage($"Language name cannot exceed {ApplicationLimits.Common.NameMaxLength} characters.");

      RuleFor(createViewModel => createViewModel.Code)
          .NotEmpty().WithMessage("ISO Code is required.")
          .Length(ApplicationLimits.Language.CodeMaxLength).WithMessage("ISO Code must be exactly 2 characters (e.g., 'en', 'tr').")
          .Matches("^[a-zA-Z]+$").WithMessage("ISO Code can only contain letters.");

      RuleFor(createViewModel => createViewModel.DisplayOrder)
          .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

      RuleFor(createViewModel => createViewModel.FlagImage)
          .NotNull().WithMessage("Flag image is required.")
          .AllowedExtensions(ApplicationLimits.Language.AllowedFlagExtensions)
          .MaxFileSize(ApplicationLimits.Language.MaxFlagSizeBytes);
   }
}
