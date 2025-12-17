using Microsoft.AspNetCore.Mvc.Rendering;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public abstract class UserFormViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IList<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
