using System.ComponentModel.DataAnnotations;

namespace PW.Admin.Web.Features.Languages.ViewModels;

public class LanguageCreateViewModel : LanguageFormViewModel
{
   [Display(Name = "Flag Icon")]
   public IFormFile? FlagImage { get; set; }
}
