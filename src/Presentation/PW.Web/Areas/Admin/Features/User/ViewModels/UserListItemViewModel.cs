namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserListItemViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
