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
        private readonly IIdentityService _identityService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserOrchestrator(
            IIdentityService identityService,
            IRoleService roleService,
            IMapper mapper)
        {
            _identityService = identityService;
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<OperationResult<UserEditViewModel>> PrepareUserEditViewModelAsync(int userId)
        {
            var userDto = await _identityService.GetUserByIdAsync(userId);

            if (userDto is null)
                return OperationResult<UserEditViewModel>.Failure("User not found!");

            var allRoles = await _roleService.GetAllRolesAsync();

            var model = _mapper.Map<UserEditViewModel>(userDto);

            model.SelectedRoles = userDto.RoleNames ?? new List<string>();

            model.AvailableRoles = allRoles
                .Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role,
                    Selected = model.SelectedRoles.Contains(role)
                })
                .ToList();

            return OperationResult<UserEditViewModel>.Success(model);
        }

        public async Task<UserListViewModel> PrepareUserListViewModelAsync()
        {
            var userDtos = await _identityService.GetAllUsersAsync();

            return new UserListViewModel
            {
                Users = _mapper.Map<List<UserListItemViewModel>>(userDtos)
            };
        }

        public async Task<OperationResult> UpdateUserAsync(UserEditViewModel model)
        {
            var dto = _mapper.Map<UserDto>(model);

            var infoResult = await _identityService.UpdateUserAsync(dto);

            if (!infoResult.Succeeded)
                return infoResult;

            var assignmentDto = new UserRoleAssignmentDto
            {
                UserId = model.Id,
                RoleNames = model.SelectedRoles ?? new List<string>()
            };

            var roleResult = await _roleService.UpdateUserRolesAsync(assignmentDto);
            if (!roleResult.Succeeded)
                return roleResult;

            if (model.ChangePassword)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    return OperationResult.Failure("Password cannot be empty when ChangePassword is enabled.");

                var passResult = await _identityService.ChangeUserPasswordAsync(model.Id, model.Password);
                if (!passResult.Succeeded)
                    return passResult;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> CreateUserAsync(UserCreateViewModel model)
        {
            int? existingUserId = await _identityService.FindByEmailAsync(model.Email);

            if (existingUserId is not null)
                return OperationResult.Failure("Email is already taken.");

            var createResult = await _identityService.CreateUserAsync(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password
            );

            if (!createResult.Succeeded)
                return OperationResult.Failure(createResult.Errors.ToArray());

            int userId = createResult.Data;

            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                var assignmentDto = new UserRoleAssignmentDto
                {
                    UserId = userId,
                    RoleNames = model.SelectedRoles
                };

                var roleResult = await _roleService.UpdateUserRolesAsync(assignmentDto);
                if (!roleResult.Succeeded)
                    return OperationResult.Failure(roleResult.Errors.ToArray());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult<UserCreateViewModel>> PrepareUserCreateViewModelAsync()
        {
            var roles = await _roleService.GetAllRolesAsync();

            var model = new UserCreateViewModel
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

            return OperationResult<UserCreateViewModel>.Success(model);
        }

        public async Task<OperationResult> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
                return OperationResult.Failure("Invalid user id.");

            var deleteResult = await _identityService.DeleteUserAsync(userId);

            if (!deleteResult.Succeeded)
                return deleteResult;

            return OperationResult.Success();
        }
    }
}
