namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{

    public class ProfileSettingsLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string Biography { get; set; }

        public IFormFile? AvatarImage { get; set; }
        public string? AvatarPath { get; set; }
        public bool RemoveAvatar { get; set; }

        public IFormFile? CoverImage { get; set; }
        public string? CoverPath { get; set; }
        public bool RemoveCover { get; set; }
    }
}
