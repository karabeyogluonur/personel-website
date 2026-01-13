using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories.Validators;

public class CategoryTranslationViewModelValidator : AbstractValidator<CategoryTranslationViewModel>
{
  public CategoryTranslationViewModelValidator()
  {
    RuleFor(translationViewModel => translationViewModel.Name)
        .MaximumLength(ApplicationLimits.Category.NameMaxLength)
        .WithMessage($"Localized Name cannot exceed {ApplicationLimits.Category.NameMaxLength} characters.");

    RuleFor(translationViewModel => translationViewModel.Description)
        .MaximumLength(ApplicationLimits.Category.DescriptionMaxLength)
        .WithMessage($"Localized Description cannot exceed {ApplicationLimits.Category.DescriptionMaxLength} characters.");
  }
}
