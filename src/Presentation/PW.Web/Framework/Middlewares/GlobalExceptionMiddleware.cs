using System.Text.Json;
using PW.Application.Common.Exceptions;

namespace PW.Web.Framework.Middlewares;

public class GlobalExceptionMiddleware
{
   private readonly RequestDelegate _next;
   private readonly IWebHostEnvironment _env;
   private readonly ILogger<GlobalExceptionMiddleware> _logger;

   public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<GlobalExceptionMiddleware> logger)
   {
      _next = next;
      _env = env;
      _logger = logger;
   }

   public async Task Invoke(HttpContext context)
   {
      try
      {
         await _next(context);
      }
      catch (Exception ex)
      {
         if (context.Response.HasStarted)
         {
            throw;
         }

         await HandleExceptionAsync(context, ex);
      }
   }

   private async Task HandleExceptionAsync(HttpContext context, Exception ex)
   {
      int statusCode;
      object responseModel;

      switch (ex)
      {
         case ValidationException ve:
            statusCode = 400;
            responseModel = new
            {
               title = ve.Message,
               status = statusCode,
               errors = ve.Errors
            };
            break;

         case UnauthorizedException ue:
            statusCode = ue.StatusCode;
            responseModel = new
            {
               title = ue.Message,
               status = statusCode
            };
            break;

         case BaseException be:
            statusCode = be.StatusCode;
            responseModel = new
            {
               title = be.Message,
               status = statusCode,
               errors = (object)null
            };
            break;

         case UnauthorizedAccessException:
            statusCode = 401;
            responseModel = new { title = "Unauthorized Access", status = statusCode };
            break;

         default:
            statusCode = 500;
            responseModel = new
            {
               title = "An unexpected error occurred.",
               status = statusCode,
               detail = _env.IsDevelopment() ? ex.Message : "Internal Server Error"
            };
            break;
      }

      context.Response.Clear();
      context.Response.StatusCode = statusCode;

      if (IsJsonRequest(context.Request))
      {
         context.Response.ContentType = "application/json";

         var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
         await context.Response.WriteAsync(JsonSerializer.Serialize(responseModel, options));
         return;
      }

      context.Response.Redirect($"/Error/{statusCode}");
   }

   private bool IsJsonRequest(HttpRequest request)
   {
      if (request.Headers["X-Requested-With"] == "XMLHttpRequest") return true;

      if (request.Headers.TryGetValue("Accept", out var acceptValues))
      {
         return acceptValues.Any(a => a.Contains("application/json"));
      }

      return false;
   }
}
