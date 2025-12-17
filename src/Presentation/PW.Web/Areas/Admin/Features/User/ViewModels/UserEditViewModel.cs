using Microsoft.AspNetCore.Mvc.Rendering;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserEditViewModel : UserFormViewModel
    {
        public int Id { get; set; }
        public bool ChangePassword { get; set; } = false;
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
