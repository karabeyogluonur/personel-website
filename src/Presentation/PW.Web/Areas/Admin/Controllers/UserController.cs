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

        public async Task<IActionResult> Edit(int id)
        {
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
                OperationResult<UserEditViewModel> result = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id);

                if (!result.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

            OperationResult updateResult = await _userOrchestrator.UpdateUserAsync(userEditViewModel);

            if (!updateResult.Succeeded)
            {
                ModelState.AddErrors(updateResult);

                OperationResult<UserEditViewModel> result = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id);

                if (!result.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

            await _notificationService.SuccessNotificationAsync("User updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            OperationResult<UserCreateViewModel> result = await _userOrchestrator.PrepareUserCreateViewModelAsync();

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                OperationResult<UserCreateViewModel> result = await _userOrchestrator.PrepareUserCreateViewModelAsync();

                if (!result.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

            OperationResult createResult = await _userOrchestrator.CreateUserAsync(model);

            if (!createResult.Succeeded)
            {
                ModelState.AddErrors(createResult);

                OperationResult<UserCreateViewModel> result = await _userOrchestrator.PrepareUserCreateViewModelAsync();

                if (!result.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

            await _notificationService.SuccessNotificationAsync("User created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            OperationResult result = await _userOrchestrator.DeleteUserAsync(id);

            if (!result.Succeeded)
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
            else
                await _notificationService.SuccessNotificationAsync("User deleted successfully.");

            return RedirectToAction(nameof(Index));
        }
    }
}
