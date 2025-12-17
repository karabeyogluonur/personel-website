using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Icon")]
        public string IconImageFileName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Doc. URL")]
        public string DocumentationUrl { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
