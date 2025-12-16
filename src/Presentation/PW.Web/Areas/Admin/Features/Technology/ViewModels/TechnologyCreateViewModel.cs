using PW.Web.Areas.Admin.Features.Language.ViewModels;

namespace PW.Web.Areas.Admin.Features.Technology.ViewModels
{
    public class TechnologyCreateViewModel : TechnologyFormViewModel
    {
        public IFormFile IconImage { get; set; } = default!;
    }
}
