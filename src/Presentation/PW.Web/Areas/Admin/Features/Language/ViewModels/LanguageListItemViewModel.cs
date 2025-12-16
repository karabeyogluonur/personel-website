namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageListItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FlagImageFileName { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsDefault { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
