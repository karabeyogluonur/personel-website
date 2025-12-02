using Microsoft.AspNetCore.Mvc;

namespace PW.Web.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }

}
