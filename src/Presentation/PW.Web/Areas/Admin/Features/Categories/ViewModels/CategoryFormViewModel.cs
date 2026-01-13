using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Categories.ViewModels;

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
