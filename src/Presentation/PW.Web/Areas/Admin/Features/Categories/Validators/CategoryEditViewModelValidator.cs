using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories.Validators;

public class CategoryEditViewModelValidator : AbstractValidator<CategoryEditViewModel>
{
   public CategoryEditViewModelValidator()
   {
      RuleFor(editViewModel => editViewModel.Id)
          .GreaterThan(0)
          .WithMessage("Invalid Category ID.");

      RuleFor(editViewModel => editViewModel.Name)
          .NotEmpty().WithMessage("Name is required.")
          .MaximumLength(ApplicationLimits.Category.NameMaxLength)
          .WithMessage($"Name cannot exceed {ApplicationLimits.Category.NameMaxLength} characters.");

      RuleFor(editViewModel => editViewModel.Description)
          .MaximumLength(ApplicationLimits.Category.DescriptionMaxLength)
          .WithMessage($"Description cannot exceed {ApplicationLimits.Category.DescriptionMaxLength} characters.");

      RuleForEach(editViewModel => editViewModel.Translations)
          .SetValidator(new CategoryTranslationViewModelValidator());
   }
}
