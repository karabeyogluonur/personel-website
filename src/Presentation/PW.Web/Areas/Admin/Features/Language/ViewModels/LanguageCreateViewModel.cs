using System.ComponentModel.DataAnnotations;

namespace PW.Web.Areas.Admin.Features.Language.ViewModels
{
    public class LanguageCreateViewModel : LanguageFormViewModel
    {
        [Display(Name = "Flag Icon")]
        public IFormFile FlagImage { get; set; } = default!;
    }
}
