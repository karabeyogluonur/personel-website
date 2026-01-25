using Microsoft.AspNetCore.Mvc;

namespace PW.Public.Web.Controllers;

public class ErrorController : Controller
{
   [Route("Error/{statusCode:int}")]
   public IActionResult HandleStatusCode(int statusCode)
   {
      Response.StatusCode = statusCode;

      return statusCode switch
      {
         404 => View("NotFound"),
         401 => View("AccessDenied"),
         403 => View("Forbidden"),
         500 => View("Error"),
         _ => View("Error")
      };
   }
}
