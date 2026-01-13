using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.User.ViewModels;

public class UserCreateViewModel : UserFormViewModel
{
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
