using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{
    public class ProfileSettingsViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Biography")]
        public string Biography { get; set; }

        [Display(Name = "Avatar")]
        public IFormFile? AvatarImage { get; set; }
        public string? AvatarPath { get; set; }
        public bool RemoveAvatar { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile? CoverImage { get; set; }
        public string? CoverPath { get; set; }
        public bool RemoveCover { get; set; }

        public IList<ProfileSettingsLocalizedViewModel> Locales { get; set; } = new List<ProfileSettingsLocalizedViewModel>();

        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
