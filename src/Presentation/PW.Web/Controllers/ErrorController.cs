using Microsoft.AspNetCore.Mvc;

namespace PW.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode:int}")]
        public IActionResult HandleStatusCode(int statusCode)
        {
            Response.StatusCode = statusCode;

            return statusCode switch
            {
                404 => View("NotFound"),
                401 => View("Unauthorized"),
                403 => View("Unauthorized"),
                500 => View("Error"),
                _ => View("Error")
            };
        }
    }
}
