using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Category.ViewModels
{
    public class CategoryListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Cover Image")]
        public string CoverImageFileName { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
