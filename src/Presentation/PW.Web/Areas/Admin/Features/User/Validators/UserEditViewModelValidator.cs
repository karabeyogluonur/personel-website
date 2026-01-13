using FluentValidation;
using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Validators;

public class UserEditViewModelValidator : AbstractValidator<UserEditViewModel>
{
    public UserEditViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid User ID.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(ApplicationLimits.User.FirstNameMaxLength)
            .WithMessage($"First name cannot exceed {ApplicationLimits.User.FirstNameMaxLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(ApplicationLimits.User.LastNameMaxLength)
            .WithMessage($"Last name cannot exceed {ApplicationLimits.User.LastNameMaxLength} characters.");

        RuleFor(x => x.SelectedRoles)
            .NotNull().WithMessage("Please select at least one role.")
            .Must(roles => roles != null && roles.Any())
            .WithMessage("Please select at least one role.");

        When(x => x.ChangePassword, () =>
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required when changing password.")
                .MinimumLength(ApplicationLimits.User.PasswordMinLength)
                .WithMessage($"Password must be at least {ApplicationLimits.User.PasswordMinLength} characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        });
    }
}
