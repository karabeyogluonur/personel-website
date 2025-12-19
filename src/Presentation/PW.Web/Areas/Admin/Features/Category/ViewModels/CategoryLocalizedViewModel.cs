using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Category.ViewModels
{
    public class CategoryLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; } = default!;

        [Display(Name = "Remove Current Cover Image")]
        public bool RemoveCurrentCoverImage { get; set; } = false;

        public string? CurrentCoverImageFileName { get; set; }


    }
}
