namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public IFormFile? FlagImage { get; set; }
        public string CurrentFlagFileName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDefault { get; set; }
    }
}
