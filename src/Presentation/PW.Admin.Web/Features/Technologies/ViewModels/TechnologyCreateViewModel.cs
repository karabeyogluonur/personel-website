using System.ComponentModel.DataAnnotations;

namespace PW.Admin.Web.Features.Technologies.ViewModels;

public class TechnologyCreateViewModel : TechnologyFormViewModel
{
   [Display(Name = "Icon Image")]
   public IFormFile? IconImage { get; set; }
}
