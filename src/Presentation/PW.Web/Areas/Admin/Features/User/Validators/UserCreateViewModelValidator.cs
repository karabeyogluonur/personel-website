using FluentValidation;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Validators
{
    public class UserCreateViewModelValidator : AbstractValidator<UserCreateViewModel>
    {
        public UserCreateViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
                .Must(x => x == null || x.Trim() == x).WithMessage("First name cannot start or end with whitespace.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
                .Must(x => x == null || x.Trim() == x).WithMessage("Last name cannot start or end with whitespace.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm the password.")
                .When(x => !string.IsNullOrEmpty(x.Password))
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.SelectedRoles)
                .NotNull().WithMessage("Please select at least one role.")
                .Must(r => r != null && r.Count > 0)
                .WithMessage("Please select at least one role.");
        }
    }
}
