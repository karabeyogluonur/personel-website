using Microsoft.AspNetCore.Mvc.Rendering;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserCreateViewModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public List<string> SelectedRoles { get; set; } = new();

        public List<SelectListItem> AvailableRoles { get; set; } = new();
    }
}
