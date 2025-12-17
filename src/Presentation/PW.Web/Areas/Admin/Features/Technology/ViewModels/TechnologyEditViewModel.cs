using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyEditViewModel : TechnologyFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "New Icon Image")]
        public IFormFile? IconImage { get; set; }

        [Display(Name = "Current Icon")]
        public string? CurrentIconFileName { get; set; }
    }
}
