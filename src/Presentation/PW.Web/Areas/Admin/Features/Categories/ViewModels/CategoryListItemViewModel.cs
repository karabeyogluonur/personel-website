namespace PW.Web.Areas.Admin.Features.Categories.ViewModels;

public class CategoryListItemViewModel
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public bool IsActive { get; set; }
   public string? CoverImageFileName { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set; }
}
