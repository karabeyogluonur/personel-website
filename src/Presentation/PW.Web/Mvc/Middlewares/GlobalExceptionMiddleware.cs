using Application.Common.Exceptions;
using System.Text.Json;

namespace PW.Web.Mvc.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            object response;

            switch (ex)
            {
                case BaseException be:
                    statusCode = be.StatusCode;
                    response = new
                    {
                        title = be.Message,
                        status = statusCode,
                        errors = ex is ValidationException ve ? ve.Errors : null
                    };
                    break;

                case UnauthorizedAccessException:
                    statusCode = 401;
                    response = new { title = "Unauthorized", status = statusCode };
                    break;

                default:
                    statusCode = 500;
                    response = new
                    {
                        title = "An unexpected error occurred.",
                        status = statusCode,
                        detail = _env.IsDevelopment() ? ex.Message : null
                    };
                    break;
            }

            if (IsJsonRequest(context.Request))
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            context.Items["ErrorException"] = ex;
            context.Items["ErrorStatusCode"] = statusCode;
            context.Response.Redirect("/Error/500");
        }

        private bool IsJsonRequest(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Accept", out var acceptValues))
                return acceptValues.Any(a => a.Contains("application/json"));

            return false;
        }
    }
}
