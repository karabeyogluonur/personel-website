using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Configurations.ViewModels;

public class ProfileSettingsTranslationViewModel
{
   public int LanguageId { get; set; }
   public string LanguageCode { get; set; } = string.Empty;

   [Display(Name = "First Name")]
   public string? FirstName { get; set; }

   [Display(Name = "Last Name")]
   public string? LastName { get; set; }

   [Display(Name = "Job Title")]
   public string? JobTitle { get; set; }

   [Display(Name = "Biography")]
   public string? Biography { get; set; }

   [Display(Name = "Avatar")]
   public IFormFile? AvatarImage { get; set; }
   public string? AvatarPath { get; set; }
   public bool RemoveAvatar { get; set; }

   [Display(Name = "Cover Image")]
   public IFormFile? CoverImage { get; set; }
   public string? CoverPath { get; set; }
   public bool RemoveCover { get; set; }
}
