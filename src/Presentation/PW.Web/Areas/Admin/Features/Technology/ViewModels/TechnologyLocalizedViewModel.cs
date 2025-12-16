namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyLocalizedViewModel
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;

        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
