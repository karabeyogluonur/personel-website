using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using PW.Application.Common.Enums;
using PW.Application.Features.Users;
using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Users.ViewModels;
using static PW.Application.Features.Users.Dtos.UserDto;

namespace PW.Admin.Web.Features.Users.Services;

public class UserOrchestrator : IUserOrchestrator
{
   private readonly IUserService _userService;
   private readonly IMapper _mapper;

   public UserOrchestrator(IUserService userService, IMapper mapper)
   {
      _userService = userService;
      _mapper = mapper;
   }

   public async Task<OperationResult<UserListViewModel>> PrepareUserListViewModelAsync()
   {
      IList<UserSummaryDto> userSummaryDtos = await _userService.GetAllUsersAsync();

      List<UserListItemViewModel> userListItemViewModels = _mapper.Map<List<UserListItemViewModel>>(userSummaryDtos);

      UserListViewModel userListViewModel = new UserListViewModel
      {
         Users = userListItemViewModels
      };

      return OperationResult<UserListViewModel>.Success(userListViewModel);
   }

   public async Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync(UserCreateViewModel? userCreateViewModel = null)
   {
      if (userCreateViewModel == null)
         userCreateViewModel = new UserCreateViewModel();

      await LoadFormReferenceDataAsync(userCreateViewModel);

      return OperationResult<UserCreateViewModel>.Success(userCreateViewModel);
   }

   public async Task<OperationResult> CreateUserAsync(UserCreateViewModel userCreateViewModel)
   {
      if (userCreateViewModel == null)
         throw new ArgumentNullException(nameof(userCreateViewModel));

      UserCreateDto userCreateDto = _mapper.Map<UserCreateDto>(userCreateViewModel);

      return await _userService.CreateUserAsync(userCreateDto);
   }

   public async Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId, UserEditViewModel? userEditViewModel = null)
   {
      if (userEditViewModel != null)
      {
         await LoadFormReferenceDataAsync(userEditViewModel);
         return OperationResult<UserEditViewModel>.Success(userEditViewModel);
      }

      UserDetailDto? userDetailDto = await _userService.GetUserByIdAsync(userId);

      if (userDetailDto == null)
         return OperationResult<UserEditViewModel>.Failure("User not found.", OperationErrorType.NotFound);

      userEditViewModel = _mapper.Map<UserEditViewModel>(userDetailDto);
      userEditViewModel.SelectedRoles = (userDetailDto.Roles ?? new List<string>()).ToList();

      await LoadFormReferenceDataAsync(userEditViewModel);

      return OperationResult<UserEditViewModel>.Success(userEditViewModel);
   }

   public async Task<OperationResult> UpdateUserAsync(UserEditViewModel userEditViewModel)
   {
      if (userEditViewModel == null)
         throw new ArgumentNullException(nameof(userEditViewModel));

      UserUpdateDto userUpdateDto = _mapper.Map<UserUpdateDto>(userEditViewModel);
      userUpdateDto.IsPasswordChangeRequested = userEditViewModel.ChangePassword;
      userUpdateDto.NewPassword = userEditViewModel.Password;
      userUpdateDto.SelectedRoles = (userEditViewModel.SelectedRoles ?? new List<string>()).ToList();

      return await _userService.UpdateUserAsync(userUpdateDto);
   }

   public async Task<OperationResult> DeleteUserAsync(int userId)
   {
      if (userId <= 0)
         return OperationResult.Failure("Invalid user identifier.", OperationErrorType.ValidationError);

      return await _userService.DeleteUserAsync(userId);
   }

   private async Task LoadFormReferenceDataAsync(UserFormViewModel userFormViewModel)
   {
      IList<string> availableRoles = await _userService.GetAllRolesAsync();

      userFormViewModel.AvailableRoles = availableRoles
          .Select((string roleName) => new SelectListItem
          {
             Text = roleName,
             Value = roleName,
             Selected = userFormViewModel.SelectedRoles.Contains(roleName)
          })
          .ToList();
   }
}
