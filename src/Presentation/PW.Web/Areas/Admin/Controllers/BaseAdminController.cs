using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Constants;

namespace PW.Web.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class BaseAdminController : Controller
    {

    }
}
