using FluentValidation;
using PW.Web.Features.Auth.ViewModels;

namespace PW.Web.Features.Auth.Validations;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
   public LoginViewModelValidator()
   {
      RuleFor(login => login.Email).NotEmpty().NotNull().EmailAddress();
      RuleFor(login => login.Password).NotEmpty().NotNull();
   }
}
