namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageListItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string FlagImageFileName { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDefault { get; set; }

        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
