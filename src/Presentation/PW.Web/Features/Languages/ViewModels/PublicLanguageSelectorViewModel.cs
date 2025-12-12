namespace PW.Web.Features.Languages.ViewModels
{
    public class PublicLanguageSelectorViewModel
    {
        public List<PublicLanguageItemViewModel> AvailableLanguages { get; set; } = new();
        public PublicLanguageItemViewModel CurrentLanguage { get; set; } = new();
    }
}
