using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Technologies.ViewModels;

public abstract class TechnologyFormViewModel
{
   [Display(Name = "Technology Name")]
   public string Name { get; set; } = string.Empty;

   [Display(Name = "Description")]
   public string Description { get; set; } = string.Empty;

   [Display(Name = "Active Status")]
   public bool IsActive { get; set; } = true;
   public IList<TechnologyTranslationViewModel> Translations { get; set; } = new List<TechnologyTranslationViewModel>();
   public IList<LanguageLookupViewModel> AvailableLanguages { get; set; } = new List<LanguageLookupViewModel>();
}
