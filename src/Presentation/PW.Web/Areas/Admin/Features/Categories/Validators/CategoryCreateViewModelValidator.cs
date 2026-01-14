using FluentValidation;

using PW.Application.Common.Constants;
using PW.Application.Common.Extensions;
using PW.Web.Areas.Admin.Features.Categories.ViewModels;

namespace PW.Web.Areas.Admin.Features.Categories.Validators;

public class CategoryCreateViewModelValidator : AbstractValidator<CategoryCreateViewModel>
{
   public CategoryCreateViewModelValidator()
   {
      RuleFor(createViewModel => createViewModel.Name)
          .NotEmpty().WithMessage("Name is required.")
          .MaximumLength(ApplicationLimits.Category.NameMaxLength)
          .WithMessage($"Name cannot exceed {ApplicationLimits.Category.NameMaxLength} characters.");

      RuleFor(createViewModel => createViewModel.Description)
          .MaximumLength(ApplicationLimits.Category.DescriptionMaxLength)
          .WithMessage($"Description cannot exceed {ApplicationLimits.Category.DescriptionMaxLength} characters.");

      RuleForEach(createViewModel => createViewModel.Translations)
          .SetValidator(new CategoryTranslationViewModelValidator());
   }
}
