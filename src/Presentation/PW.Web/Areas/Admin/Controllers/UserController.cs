using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.User.Services;
using PW.Web.Areas.Admin.Features.User.ViewModels;
using PW.Web.Extensions;

namespace PW.Web.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserOrchestrator _userOrchestrator;
        private readonly INotificationService _notificationService;

        public UserController(
            IUserOrchestrator userOrchestrator,
            INotificationService notificationService)
        {
            _userOrchestrator = userOrchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OperationResult<UserListViewModel> result = await _userOrchestrator.PrepareUserListViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return View(new UserListViewModel());
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OperationResult<UserCreateViewModel> result = await _userOrchestrator.PrepareUserCreateViewModelAsync();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel userCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<UserCreateViewModel> reloadResult = await _userOrchestrator.PrepareUserCreateViewModelAsync(userCreateViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _userOrchestrator.CreateUserAsync(userCreateViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("User created successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not create user.");

            OperationResult<UserCreateViewModel> errorReloadResult = await _userOrchestrator.PrepareUserCreateViewModelAsync(userCreateViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction(nameof(Index));

            OperationResult<UserEditViewModel> result = await _userOrchestrator.PrepareUserEditViewModelAsync(id);

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<UserEditViewModel> reloadResult = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id, userEditViewModel);
                return View(reloadResult.Data);
            }

            OperationResult result = await _userOrchestrator.UpdateUserAsync(userEditViewModel);

            if (result.Succeeded)
            {
                await _notificationService.SuccessNotificationAsync("User updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddErrors(result);
            await _notificationService.ErrorNotificationAsync("Could not update user.");

            OperationResult<UserEditViewModel> errorReloadResult = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id, userEditViewModel);
            return View(errorReloadResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            OperationResult result = await _userOrchestrator.DeleteUserAsync(id);

            if (result.Succeeded)
                await _notificationService.SuccessNotificationAsync("User deleted successfully.");
            else
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault() ?? "Error deleting user.");

            return RedirectToAction(nameof(Index));
        }
    }
}
