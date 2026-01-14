using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technologies.Validators;

public class TechnologyEditViewModelValidator : AbstractValidator<TechnologyEditViewModel>
{
   public TechnologyEditViewModelValidator()
   {
      RuleFor(editViewModel => editViewModel.Id)
          .GreaterThan(0).WithMessage("Invalid Technology ID.");

      RuleFor(editViewModel => editViewModel.Name)
          .NotEmpty().WithMessage("Name is required.")
          .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
          .WithMessage($"Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

      RuleFor(editViewModel => editViewModel.Description)
          .NotEmpty().WithMessage("Description is required.")
          .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
          .WithMessage($"Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");

      RuleFor(editViewModel => editViewModel.IconImage)
          .AllowedExtensions(ApplicationLimits.Technology.AllowedIconExtensions)
          .MaxFileSize(ApplicationLimits.Technology.MaxIconSizeBytes);

      RuleForEach(editViewModel => editViewModel.Translations)
          .SetValidator(new TechnologyTranslationViewModelValidator());
   }
}
