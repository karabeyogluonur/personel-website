namespace PW.Web.Areas.Admin.Features.User.ViewModels
{
    public class UserListItemViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public List<string> Roles { get; set; }
    }
}
