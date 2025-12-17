using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PW.Web.Areas.Admin.Features.Configuration.ViewModels
{
    public class ProfileSettingsLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Display(Name = "Biography")]
        public string Biography { get; set; } = string.Empty;

        [Display(Name = "Avatar")]
        public IFormFile? AvatarImage { get; set; }
        public string? AvatarPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveAvatar { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile? CoverImage { get; set; }
        public string? CoverPath { get; set; }

        [Display(Name = "Remove current")]
        public bool RemoveCover { get; set; }
    }
}
