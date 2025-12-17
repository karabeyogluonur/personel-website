using Microsoft.AspNetCore.Mvc.Rendering;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserCreateViewModel : UserFormViewModel
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
