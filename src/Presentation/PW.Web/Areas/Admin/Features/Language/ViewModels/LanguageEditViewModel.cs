namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageEditViewModel : LanguageFormViewModel
    {
        public int Id { get; set; }
        public IFormFile? FlagImage { get; set; }
        public string? CurrentFlagFileName { get; set; }
    }
}
