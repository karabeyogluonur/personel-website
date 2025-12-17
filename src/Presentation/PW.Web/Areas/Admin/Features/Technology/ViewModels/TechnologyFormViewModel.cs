using System.ComponentModel.DataAnnotations;
using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public abstract class TechnologyFormViewModel
    {
        [Display(Name = "Technology Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Documentation URL")]
        public string DocumentationUrl { get; set; } = string.Empty;

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        public IList<TechnologyLocalizedViewModel> Locales { get; set; } = new List<TechnologyLocalizedViewModel>();
        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
