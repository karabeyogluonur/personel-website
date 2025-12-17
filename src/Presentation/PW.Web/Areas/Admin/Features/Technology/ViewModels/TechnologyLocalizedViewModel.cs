using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
