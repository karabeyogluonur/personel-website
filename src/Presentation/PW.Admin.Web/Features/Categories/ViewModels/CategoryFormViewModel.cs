using System.ComponentModel.DataAnnotations;
using PW.Admin.Web.Features.Common.Models;

namespace PW.Admin.Web.Features.Categories.ViewModels;

public abstract class CategoryFormViewModel
{
   [Display(Name = "Name (Standard)")]
   public string Name { get; set; } = string.Empty;

   [Display(Name = "Description (Standard)")]
   public string? Description { get; set; }

   [Display(Name = "Active Status")]
   public bool IsActive { get; set; } = true;
   public IList<CategoryTranslationViewModel> Translations { get; set; } = new List<CategoryTranslationViewModel>();
   public IList<LanguageLookupViewModel> AvailableLanguages { get; set; } = new List<LanguageLookupViewModel>();
}
