using System.ComponentModel.DataAnnotations;
using PW.Admin.Web.Features.Common.Models;

namespace PW.Admin.Web.Features.Tags.ViewModels;

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

