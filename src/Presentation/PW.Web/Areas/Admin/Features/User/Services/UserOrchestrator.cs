using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Web.Areas.Admin.Features.User.ViewModels;


namespace PW.Web.Areas.Admin.Features.User.Services
{
    public class UserOrchestrator : IUserOrchestrator
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserOrchestrator(
            IUserService userService,
            IRoleService roleService,
            IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId)
        {
            UserDto userDto = await _userService.GetUserByIdAsync(userId);

            if (userDto is null)
                return OperationResult<UserEditViewModel>.Failure("User not found!");

            List<string> allRoles = await _roleService.GetAllRolesAsync();

            UserEditViewModel userEditViewModel = _mapper.Map<UserEditViewModel>(userDto);

            userEditViewModel.SelectedRoles = userDto.Roles ?? new List<string>();

            userEditViewModel.AvailableRoles = allRoles
                .Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role,
                    Selected = userEditViewModel.SelectedRoles.Contains(role)
                })
                .ToList();

            return OperationResult<UserEditViewModel>.Success(userEditViewModel);
        }

        public async Task<UserListViewModel> PrepareUserListViewModelAsync()
        {
            var userDtos = await _userService.GetAllUsersAsync();

            return new UserListViewModel
            {
                Users = _mapper.Map<List<UserListItemViewModel>>(userDtos)
            };
        }

        public async Task<OperationResult> UpdateUserAsync(UserEditViewModel userEditViewModel)
        {
            UserDto userDto = _mapper.Map<UserDto>(userEditViewModel);

            OperationResult infoResult = await _userService.UpdateUserAsync(userEditViewModel.Id, userDto);

            if (!infoResult.Succeeded)
                return infoResult;

            UserRoleAssignmentDto userRoleAssignmentDto = new UserRoleAssignmentDto
            {
                UserId = userEditViewModel.Id,
                RoleNames = userEditViewModel.SelectedRoles ?? new List<string>()
            };

            OperationResult roleResult = await _roleService.UpdateUserRolesAsync(userRoleAssignmentDto);
            if (!roleResult.Succeeded)
                return roleResult;

            if (userEditViewModel.ChangePassword)
            {
                if (string.IsNullOrWhiteSpace(userEditViewModel.Password))
                    return OperationResult.Failure("Password cannot be empty when ChangePassword is enabled.");

                SetPasswordDto setPasswordDto = new SetPasswordDto
                {
                    UserId = userEditViewModel.Id,
                    NewPassword = userEditViewModel.Password
                };

                OperationResult passResult = await _userService.AdminResetUserPasswordAsync(setPasswordDto);
                if (!passResult.Succeeded)
                    return passResult;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> CreateUserAsync(UserCreateViewModel model)
        {
            UserDto existingUser = await _userService.GetUserByEmailAsync(model.Email);

            if (existingUser is not null)
                return OperationResult.Failure("Email is already taken.");

            CreateUserDto createUserDto = new CreateUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Roles = model.SelectedRoles
            };

            OperationResult createUserResult = await _userService.CreateUserAsync(createUserDto);

            if (!createUserResult.Succeeded)
                return OperationResult.Failure(createUserResult.Errors.ToArray());

            return OperationResult.Success();
        }

        public async Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync()
        {
            List<string> roles = await _roleService.GetAllRolesAsync();

            UserCreateViewModel userCreateViewModel = new UserCreateViewModel
            {
                AvailableRoles = roles
                    .Select(role => new SelectListItem
                    {
                        Text = role,
                        Value = role
                    })
                    .ToList(),
                SelectedRoles = new List<string>()
            };

            return OperationResult<UserCreateViewModel>.Success(userCreateViewModel);
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
