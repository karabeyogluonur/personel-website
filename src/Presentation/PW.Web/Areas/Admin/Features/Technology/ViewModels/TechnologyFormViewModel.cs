using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public abstract class TechnologyFormViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DocumentationUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public IList<TechnologyLocalizedViewModel> Locales { get; set; } = new List<TechnologyLocalizedViewModel>();
        public IList<LanguageListItemViewModel> AvailableLanguages { get; set; } = new List<LanguageListItemViewModel>();
    }
}
