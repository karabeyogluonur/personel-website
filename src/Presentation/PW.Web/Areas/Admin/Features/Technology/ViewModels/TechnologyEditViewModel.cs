using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyEditViewModel : TechnologyFormViewModel
    {
        public int Id { get; set; }
        public IFormFile? IconImage { get; set; }
        public string? CurrentIconFileName { get; set; }
    }
}
