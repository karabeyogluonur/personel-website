using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Utilities.Results;
using PW.Admin.Web.Services.Messages;

namespace PW.Admin.Web.Controllers;

[Authorize]
public abstract class BaseAdminController : Controller
{
   protected INotificationService _notificationService =>
       HttpContext.RequestServices.GetService<INotificationService>()!;

   protected async Task<IActionResult> HandleFormAsync<TViewModel>(
       TViewModel viewModel,
       Func<Task<OperationResult>> workAction,
       Func<Task<OperationResult<TViewModel>>> reloadAction,
       string successMessage,
       string redirectTo = "Index")
   {
      if (!ModelState.IsValid)
      {
         await _notificationService.ErrorNotificationAsync("Please correct the errors in the form.");
         var reloadResult = await reloadAction();
         return View(reloadResult.Data ?? viewModel);
      }

      var result = await workAction();

      if (result.Succeeded)
      {
         await _notificationService.SuccessNotificationAsync(successMessage);
         return RedirectToAction(redirectTo);
      }

      if (result.Errors.Any(error => error.Type == OperationErrorType.NotFound))
         return NotFound();

      foreach (var error in result.Errors)
      {
         string key = string.IsNullOrEmpty(error.PropertyName) ? string.Empty : error.PropertyName;
         ModelState.AddModelError(key, error.Message);
      }

      await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message ?? "Operation failed.");

      var errorReloadResult = await reloadAction();
      return View(errorReloadResult.Data ?? viewModel);
   }

   protected async Task<IActionResult> HandleDeleteAsync(
       Func<Task<OperationResult>> deleteAction,
       string successMessage = "Deletion successful.",
       string redirectTo = "Index")
   {
      var result = await deleteAction();

      if (result.Succeeded)
      {
         await _notificationService.SuccessNotificationAsync(successMessage);
      }
      else
      {
         var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "An error occurred during deletion.";
         await _notificationService.ErrorNotificationAsync(errorMessage);
      }

      return RedirectToAction(redirectTo);
   }
}
