namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconImageFileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DocumentationUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
