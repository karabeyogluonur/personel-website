using FluentValidation;

using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technologies.Validators;

public class TechnologyTranslationViewModelValidator : AbstractValidator<TechnologyTranslationViewModel>
{
   public TechnologyTranslationViewModelValidator()
   {
      RuleFor(translationViewModel => translationViewModel.Name)
          .MaximumLength(ApplicationLimits.Technology.NameMaxLength)
          .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Technology.NameMaxLength} characters.");

      RuleFor(translationViewModel => translationViewModel.Description)
          .MaximumLength(ApplicationLimits.Technology.DescriptionMaxLength)
          .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Technology.DescriptionMaxLength} characters.");
   }
}
