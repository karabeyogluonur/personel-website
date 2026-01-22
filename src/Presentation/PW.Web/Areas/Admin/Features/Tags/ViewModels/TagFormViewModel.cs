using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Common.Models;

namespace PW.Web.Areas.Admin.Features.Tags.ViewModels;

public abstract class TagFormViewModel
{
    [Display(Name = "Name (Standard)")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description (Standard)")]
    public string? Description { get; set; }

    [Display(Name = "Badge Color")]
    public string? ColorHex { get; set; }

    [Display(Name = "Active Status")]
    public bool IsActive { get; set; } = true;
    public IList<TagTranslationViewModel> Translations { get; set; } = new List<TagTranslationViewModel>();
    public IList<LanguageLookupViewModel> AvailableLanguages { get; set; } = new List<LanguageLookupViewModel>();
}

