using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public abstract class LanguageFormViewModel
    {
        [Display(Name = "Language Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ISO Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "Publish Language")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Set as Default")]
        public bool IsDefault { get; set; } = false;
    }
}
