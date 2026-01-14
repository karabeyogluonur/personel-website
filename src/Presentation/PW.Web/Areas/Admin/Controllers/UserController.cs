using Microsoft.AspNetCore.Mvc;
using PW.Web.Areas.Admin.Features.User.Services;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Controllers;

public class UserController : BaseAdminController
{
   private readonly IUserOrchestrator _userOrchestrator;

   public UserController(IUserOrchestrator userOrchestrator)
   {
      _userOrchestrator = userOrchestrator;
   }

   [HttpGet]
   public async Task<IActionResult> Index()
   {
      var result = await _userOrchestrator.PrepareUserListViewModelAsync();

      if (result.IsFailure)
      {
         await _notificationService.ErrorNotificationAsync("An error occurred while loading users.");
         return View(new UserListViewModel());
      }

      return View(result.Data);
   }

   [HttpGet]
   public async Task<IActionResult> Create()
   {
      var result = await _userOrchestrator.PrepareUserCreateViewModelAsync();
      return View(result.Data);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Create(UserCreateViewModel userCreateViewModel)
   {
      return await HandleFormAsync(
          viewModel: userCreateViewModel,
          workAction: () => _userOrchestrator.CreateUserAsync(userCreateViewModel),
          reloadAction: () => _userOrchestrator.PrepareUserCreateViewModelAsync(userCreateViewModel),
          successMessage: "User created successfully.",
          redirectTo: nameof(Index)
      );
   }

   [HttpGet]
   public async Task<IActionResult> Edit(int id)
   {
      if (id <= 0)
         return RedirectToAction(nameof(Index));

      var result = await _userOrchestrator.PrepareUserEditViewModelAsync(id);

      if (result.IsFailure)
      {
         await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault()?.Message);
         return RedirectToAction(nameof(Index));
      }

      return View(result.Data);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Edit(UserEditViewModel userEditViewModel)
   {
      return await HandleFormAsync(
          viewModel: userEditViewModel,
          workAction: () => _userOrchestrator.UpdateUserAsync(userEditViewModel),
          reloadAction: () => _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id, userEditViewModel),
          successMessage: "User updated successfully.",
          redirectTo: nameof(Index)
      );
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Delete(int id)
   {
      return await HandleDeleteAsync(
          deleteAction: () => _userOrchestrator.DeleteUserAsync(id),
          successMessage: "User deleted successfully."
      );
   }
}
