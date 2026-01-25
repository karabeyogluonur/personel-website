using Microsoft.AspNetCore.Mvc;

namespace PW.Admin.Web.Controllers;

public class HomeController : BaseAdminController
{
   public async Task<IActionResult> Index()
   {
      return View();
   }
}
