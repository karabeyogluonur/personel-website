using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Category.ViewModels
{
    public class CategoryEditViewModel : CategoryFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Current Cover Image")]
        public string? CurrentCoverImageFileName { get; set; }

        [Display(Name = "Remove Current Cover Image")]
        public bool RemoveCurrentCoverImage { get; set; } = false;
    }
}
