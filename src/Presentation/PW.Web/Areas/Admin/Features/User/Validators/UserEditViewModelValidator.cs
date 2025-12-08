using FluentValidation;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Validators
{
    public class UserEditViewModelValidator : AbstractValidator<UserEditViewModel>
    {
        public UserEditViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.SelectedRoles)
                .NotNull().WithMessage("Please select at least one role.")
                .Must(list => list.Any()).WithMessage("Please select at least one role.");

            When(x => x.ChangePassword, () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required when changing password.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Confirm password is required.")
                    .Equal(x => x.Password).WithMessage("Passwords do not match.");
            });
        }
    }

}

