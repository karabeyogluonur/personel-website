using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{
    public class ProfileSettingsViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public IFormFile? AvatarImage { get; set; }
        public string? AvatarPath { get; set; }
        public bool RemoveAvatar { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? CoverPath { get; set; }
        public bool RemoveCover { get; set; }

        public IList<ProfileSettingsLocalizedViewModel> Locales { get; set; } = new List<ProfileSettingsLocalizedViewModel>();

        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
