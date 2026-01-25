using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Users.ViewModels;

namespace PW.Admin.Web.Features.Users.Services;

public interface IUserOrchestrator
{
   Task<OperationResult<UserListViewModel>> PrepareUserListViewModelAsync();

   Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync(UserCreateViewModel? userCreateViewModel = null);

   Task<OperationResult> CreateUserAsync(UserCreateViewModel userCreateViewModel);

   Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId, UserEditViewModel? userEditViewModel = null);

   Task<OperationResult> UpdateUserAsync(UserEditViewModel userEditViewModel);

   Task<OperationResult> DeleteUserAsync(int userId);
}
