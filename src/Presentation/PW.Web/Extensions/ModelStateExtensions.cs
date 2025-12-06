using Microsoft.AspNetCore.Mvc.ModelBinding;
using PW.Application.Common.Models;

namespace PW.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddErrors(this ModelStateDictionary modelState, OperationResult result)
        {
            foreach (var error in result.Errors)
                modelState.AddModelError(string.Empty, error);
        }
    }

}
