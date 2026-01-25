using System.ComponentModel.DataAnnotations;

namespace PW.Admin.Web.Features.Technologies.ViewModels;

public class TechnologyTranslationViewModel
{
   public int LanguageId { get; set; }
   public string LanguageCode { get; set; } = string.Empty;

   [Display(Name = "Name")]
   public string? Name { get; set; }

   [Display(Name = "Description")]
   public string? Description { get; set; }
}
