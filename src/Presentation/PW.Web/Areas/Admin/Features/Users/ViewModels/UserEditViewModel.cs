using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Users.ViewModels;

public class UserEditViewModel : UserFormViewModel
{
   public int Id { get; set; }

   [Display(Name = "Change Password")]
   public bool ChangePassword { get; set; } = false;

   [Display(Name = "New Password")]
   public string? Password { get; set; }

   [Display(Name = "Confirm Password")]
   public string? ConfirmPassword { get; set; }
}
