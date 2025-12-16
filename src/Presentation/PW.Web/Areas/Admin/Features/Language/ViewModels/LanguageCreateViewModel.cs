namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageCreateViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public IFormFile? FlagImage { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = true;
        public bool IsDefault { get; set; } = false;
    }
}
