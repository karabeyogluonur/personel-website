using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Common.Models;

public class LanguageLookupViewModel
{
    public int Id { get; set; }

    [Display(Name = "ISO Code")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Language Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Flag")]
    public string? FlagImageFileName { get; set; }

    [Display(Name = "Default")]
    public bool IsDefault { get; set; }
}
