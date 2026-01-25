using Microsoft.AspNetCore.Mvc;

namespace PW.Public.Web.Controllers;

public class HomeController : BasePublicController
{
   public HomeController()
   {
   }

   public IActionResult Index()
   {
      return View();
   }

}
