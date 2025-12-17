using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public abstract class UserFormViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "User Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();

        public IList<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
