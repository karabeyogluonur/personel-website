using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Languages.ViewModels;

public class LanguageListItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "ISO Code")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Language Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Flag")]
    public string? FlagImageFileName { get; set; }

    [Display(Name = "Status")]
    public bool IsPublished { get; set; }

    [Display(Name = "Default")]
    public bool IsDefault { get; set; }

    [Display(Name = "Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Updated At")]
    public DateTime? UpdatedAt { get; set; }
}
