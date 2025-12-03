using Microsoft.AspNetCore.Mvc;

namespace PW.Web.Controllers;

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
