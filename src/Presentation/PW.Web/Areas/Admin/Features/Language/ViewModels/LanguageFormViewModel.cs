namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public abstract class LanguageFormViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = true;
        public bool IsDefault { get; set; } = false;
    }
}
