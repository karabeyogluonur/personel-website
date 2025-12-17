using FluentValidation;
using PW.Application.Common.Constants;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Validators
{
    public class UserCreateViewModelValidator : AbstractValidator<UserCreateViewModel>
    {
        public UserCreateViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(ApplicationLimits.User.FirstNameMaxLength)
                .WithMessage($"First name cannot exceed {ApplicationLimits.User.FirstNameMaxLength} characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(ApplicationLimits.User.LastNameMaxLength)
                .WithMessage($"Last name cannot exceed {ApplicationLimits.User.LastNameMaxLength} characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(ApplicationLimits.User.EmailMaxLength)
                .WithMessage($"Email cannot exceed {ApplicationLimits.User.EmailMaxLength} characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(ApplicationLimits.User.PasswordMinLength)
                .WithMessage($"Password must be at least {ApplicationLimits.User.PasswordMinLength} characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm the password.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.SelectedRoles)
                .NotNull().WithMessage("Please select at least one role.")
                .Must(roles => roles != null && roles.Any())
                .WithMessage("Please select at least one role.");
        }
    }
}
