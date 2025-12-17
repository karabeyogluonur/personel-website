using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Interfaces.Repositories;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Areas.Admin.Features.User.ViewModels;

namespace PW.Web.Areas.Admin.Features.User.Services
{
    public class UserOrchestrator : IUserOrchestrator
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserOrchestrator(
            IUserService userService,
            IRoleService roleService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task LoadFormReferenceDataAsync(UserFormViewModel userFormViewModel)
        {
            List<string> allRoles = await _roleService.GetAllRolesAsync();

            userFormViewModel.AvailableRoles = allRoles
                .Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role,
                    Selected = userFormViewModel.SelectedRoles.Contains(role)
                })
                .ToList();
        }

        public async Task<OperationResult<UserListViewModel>> PrepareUserListViewModelAsync()
        {
            List<UserDto> userDtos = await _userService.GetAllUsersAsync();

            UserListViewModel userListViewModel = new UserListViewModel
            {
                Users = _mapper.Map<List<UserListItemViewModel>>(userDtos)
            };

            return OperationResult<UserListViewModel>.Success(userListViewModel);
        }

        public async Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync(UserCreateViewModel? userCreateViewModel = null)
        {
            if (userCreateViewModel != null)
            {
                await LoadFormReferenceDataAsync(userCreateViewModel);
                return OperationResult<UserCreateViewModel>.Success(userCreateViewModel);
            }

            userCreateViewModel = new UserCreateViewModel();
            await LoadFormReferenceDataAsync(userCreateViewModel);

            return OperationResult<UserCreateViewModel>.Success(userCreateViewModel);
        }

        public async Task<OperationResult> CreateUserAsync(UserCreateViewModel userCreateViewModel)
        {
            UserDto existingUser = await _userService.GetUserByEmailAsync(userCreateViewModel.Email);

            if (existingUser is not null)
                return OperationResult.Failure("Email is already taken.");

            CreateUserDto createUserDto = _mapper.Map<CreateUserDto>(userCreateViewModel);

            OperationResult createUserResult = await _userService.CreateUserAsync(createUserDto);

            if (!createUserResult.Succeeded)
                return OperationResult.Failure(createUserResult.Errors.ToArray());

            return OperationResult.Success();
        }

        public async Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId, UserEditViewModel? userEditViewModel = null)
        {
            if (userEditViewModel != null)
            {
                await LoadFormReferenceDataAsync(userEditViewModel);
                return OperationResult<UserEditViewModel>.Success(userEditViewModel);
            }

            UserDto userDto = await _userService.GetUserByIdAsync(userId);

            if (userDto is null)
                return OperationResult<UserEditViewModel>.Failure("User not found!");

            userEditViewModel = _mapper.Map<UserEditViewModel>(userDto);

            userEditViewModel.SelectedRoles = userDto.Roles ?? new List<string>();

            await LoadFormReferenceDataAsync(userEditViewModel);

            return OperationResult<UserEditViewModel>.Success(userEditViewModel);
        }

        public async Task<OperationResult> UpdateUserAsync(UserEditViewModel userEditViewModel)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                UserDto userDto = _mapper.Map<UserDto>(userEditViewModel);
                OperationResult updateResult = await _userService.UpdateUserAsync(userEditViewModel.Id, userDto);

                if (!updateResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return updateResult;
                }

                UserRoleAssignmentDto userRoleAssignmentDto = new UserRoleAssignmentDto
                {
                    UserId = userEditViewModel.Id,
                    RoleNames = userEditViewModel.SelectedRoles.ToList() ?? new List<string>()
                };

                OperationResult roleResult = await _roleService.UpdateUserRolesAsync(userRoleAssignmentDto);

                if (!roleResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return roleResult;
                }

                if (userEditViewModel.ChangePassword)
                {
                    if (string.IsNullOrWhiteSpace(userEditViewModel.Password))
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return OperationResult.Failure("Password cannot be empty when ChangePassword is enabled.");
                    }

                    SetPasswordDto setPasswordDto = new SetPasswordDto
                    {
                        UserId = userEditViewModel.Id,
                        NewPassword = userEditViewModel.Password
                    };

                    OperationResult passwordResult = await _userService.AdminResetUserPasswordAsync(setPasswordDto);

                    if (!passwordResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return passwordResult;
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return OperationResult.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
                return OperationResult.Failure("Invalid user id.");

            OperationResult deleteResult = await _userService.DeleteUserAsync(userId);

            if (!deleteResult.Succeeded)
                return deleteResult;

            return OperationResult.Success();
        }
    }
}
