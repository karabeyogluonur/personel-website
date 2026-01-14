using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Languages.ViewModels;

public class LanguageEditViewModel : LanguageFormViewModel
{
   public int Id { get; set; }

   [Display(Name = "New Flag Icon")]
   public IFormFile? FlagImage { get; set; }

   [Display(Name = "Current Icon")]
   public string? CurrentFlagFileName { get; set; }

   [Display(Name = "Remove Current Icon")]
   public bool RemoveFlagImage { get; set; }
}
