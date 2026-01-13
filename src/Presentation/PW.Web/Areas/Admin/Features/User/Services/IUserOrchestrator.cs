using PW.Application.Models;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Services;

public interface IUserOrchestrator
{
    Task<OperationResult<UserListViewModel>> PrepareUserListViewModelAsync();

    Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync(UserCreateViewModel? userCreateViewModel = null);

    Task<OperationResult> CreateUserAsync(UserCreateViewModel userCreateViewModel);

    Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId, UserEditViewModel? userEditViewModel = null);

    Task<OperationResult> UpdateUserAsync(UserEditViewModel userEditViewModel);

    Task<OperationResult> DeleteUserAsync(int userId);
}
