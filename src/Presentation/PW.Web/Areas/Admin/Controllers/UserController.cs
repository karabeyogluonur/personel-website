using Microsoft.AspNetCore.Mvc;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Messages;
using PW.Web.Areas.Admin.Features.User.Services;
using PW.Web.Areas.Admin.Features.User.ViewModels;

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
            return View(await _userOrchestrator.PrepareUserListViewModelAsync());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var operationResult = await _userOrchestrator.PrepareUserEditViewModelAsync(id);

            if (!operationResult.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(operationResult.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            return View(operationResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                var prepareResult = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id);

                if (!prepareResult.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(prepareResult.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(prepareResult.Data);
            }

            OperationResult updateResult = await _userOrchestrator.UpdateUserAsync(userEditViewModel);

            if (!updateResult.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(updateResult.Errors.FirstOrDefault());

                var prepareResult = await _userOrchestrator.PrepareUserEditViewModelAsync(userEditViewModel.Id);

                if (prepareResult.Succeeded)
                    return View(prepareResult.Data);

                return RedirectToAction(nameof(Index));
            }
            await _notificationService.SuccessNotificationAsync("User updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            var result = await _userOrchestrator.PrepareUserCreateViewModelAsync();

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
                var result = await _userOrchestrator.PrepareUserCreateViewModelAsync();

                if (!result.Succeeded)
                {
                    await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

            var createResult = await _userOrchestrator.CreateUserAsync(model);

            if (!createResult.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(createResult.Errors.FirstOrDefault());

                var result = await _userOrchestrator.PrepareUserCreateViewModelAsync();
                return View(result.Data);
            }

            await _notificationService.SuccessNotificationAsync("User created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userOrchestrator.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                await _notificationService.ErrorNotificationAsync(result.Errors.FirstOrDefault());
                return RedirectToAction(nameof(Index));
            }

            await _notificationService.SuccessNotificationAsync("User deleted successfully.");
            return RedirectToAction(nameof(Index));
        }


    }

}
