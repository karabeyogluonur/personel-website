using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageEditViewModel : LanguageFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "New Flag Icon")]
        public IFormFile? FlagImage { get; set; }

        [Display(Name = "Current Icon")]
        public string? CurrentFlagFileName { get; set; }
    }
}
