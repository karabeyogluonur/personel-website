using FluentValidation;
using PW.Admin.Web.Features.Auth.ViewModels;

namespace PW.Admin.Web.Features.Auth.Validations;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
   public LoginViewModelValidator()
   {
      RuleFor(login => login.Email).NotEmpty().NotNull().EmailAddress();
      RuleFor(login => login.Password).NotEmpty().NotNull();
   }
}
