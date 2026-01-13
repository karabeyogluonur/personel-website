using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Technologies.ViewModels;

public class TechnologyCreateViewModel : TechnologyFormViewModel
{
    [Display(Name = "Icon Image")]
    public IFormFile? IconImage { get; set; }
}
