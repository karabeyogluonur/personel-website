using PW.Application.Common.Models;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Services
{
    public interface IUserOrchestrator
    {
        Task<UserListViewModel> PrepareUserListViewModelAsync();
        Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId);
        Task<OperationResult> UpdateUserAsync(UserEditViewModel model);
        Task<OperationResult> CreateUserAsync(UserCreateViewModel model);
        Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync();
        Task<OperationResult> DeleteUserAsync(int userId);
    }
}
