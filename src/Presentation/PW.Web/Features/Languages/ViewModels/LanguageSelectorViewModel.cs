namespace PW.Web.Features.Languages.ViewModels;

public class LanguageSelectorViewModel
{
    public List<LanguageSelectorItemViewModel> AvailableLanguages { get; set; } = new();
    public LanguageSelectorItemViewModel CurrentLanguage { get; set; } = new();
}
