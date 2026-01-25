using System.ComponentModel.DataAnnotations;

namespace PW.Admin.Web.Features.Categories.ViewModels;

public class CategoryTranslationViewModel
{
   public int LanguageId { get; set; }
   public string LanguageCode { get; set; } = string.Empty;

   [Display(Name = "Name")]
   public string? Name { get; set; }

   [Display(Name = "Description")]
   public string? Description { get; set; }
}
