using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Technologies.ViewModels;

public class TechnologyListItemViewModel
{
   public int Id { get; set; }

   [Display(Name = "Name")]
   public string Name { get; set; } = string.Empty;

   [Display(Name = "Icon")]
   public string? IconImageFileName { get; set; }

   [Display(Name = "Description")]
   public string? Description { get; set; }

   [Display(Name = "Status")]
   public bool IsActive { get; set; }

   [Display(Name = "Created At")]
   public DateTime CreatedAt { get; set; }
}
