using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Category.ViewModels
{
    public abstract class CategoryFormViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        public IList<CategoryLocalizedViewModel> Locales { get; set; } = new List<CategoryLocalizedViewModel>();
        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
