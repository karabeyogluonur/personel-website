using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Roles")]
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
