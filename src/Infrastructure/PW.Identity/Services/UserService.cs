using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Models;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;

namespace PW.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult<int>> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (createUserDto is null)
                throw new ArgumentNullException(nameof(createUserDto));

            if (string.IsNullOrWhiteSpace(createUserDto.Email))
                return OperationResult<int>.Failure("Email is required.", OperationErrorType.ValidationError);

            var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);

            if (existingUser is not null)
                return OperationResult<int>.Failure($"Email '{createUserDto.Email}' is already registered.", OperationErrorType.Conflict);

            var applicationUser = new ApplicationUser
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                UserName = createUserDto.Email,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(applicationUser, createUserDto.Password);

            if (!identityResult.Succeeded)
            {
                var errorMessage = identityResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "User creation failed.";
                return OperationResult<int>.Failure(errorMessage, OperationErrorType.ValidationError);
            }

            if (createUserDto.Roles is not null && createUserDto.Roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(applicationUser, createUserDto.Roles);
                if (!roleResult.Succeeded)
                    return OperationResult<int>.Failure("User created but roles could not be assigned.", OperationErrorType.Technical);
            }

            return OperationResult<int>.Success(applicationUser.Id);
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            if (userId <= 0) return null;

            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return null;

            var roles = await _userManager.GetRolesAsync(applicationUser);

            return new UserDto
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email,
                Roles = roles.ToList()
            };
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser is null)
                return null;

            var roles = await _userManager.GetRolesAsync(applicationUser);

            return new UserDto
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email,
                Roles = roles.ToList()
            };
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var applicationUsers = await _userManager.Users.AsNoTracking().ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var applicationUser in applicationUsers)
            {
                var userRoleNames = await _userManager.GetRolesAsync(applicationUser);

                userDtos.Add(new UserDto
                {
                    Id = applicationUser.Id,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    Email = applicationUser.Email,
                    Roles = userRoleNames.ToList()
                });
            }

            return userDtos;
        }

        public async Task<OperationResult> UpdateUserAsync(int userId, UserDto userDto)
        {
            if (userId <= 0)
                return OperationResult.Failure("Invalid user ID.", OperationErrorType.ValidationError);

            if (userDto is null)
                throw new ArgumentNullException(nameof(userDto));

            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

            if (!string.Equals(applicationUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailCheck = await _userManager.FindByEmailAsync(userDto.Email);

                if (emailCheck is not null)
                    return OperationResult.Failure($"Email '{userDto.Email}' is already taken by another user.", OperationErrorType.Conflict);

                applicationUser.Email = userDto.Email;
                applicationUser.UserName = userDto.Email;
            }

            applicationUser.FirstName = userDto.FirstName;
            applicationUser.LastName = userDto.LastName;

            var identityResult = await _userManager.UpdateAsync(applicationUser);

            if (!identityResult.Succeeded)
                return OperationResult.Failure("Failed to update user profile.", OperationErrorType.Technical);

            return OperationResult.Success();
        }

        public async Task<OperationResult> AdminResetUserPasswordAsync(SetPasswordDto setPasswordDto)
        {
            if (setPasswordDto is null)
                throw new ArgumentNullException(nameof(setPasswordDto));

            if (setPasswordDto.UserId <= 0)
                return OperationResult.Failure("Invalid user ID.", OperationErrorType.ValidationError);

            if (string.IsNullOrWhiteSpace(setPasswordDto.NewPassword))
                return OperationResult.Failure("New password cannot be empty.", OperationErrorType.ValidationError);

            var applicationUser = await _userManager.FindByIdAsync(setPasswordDto.UserId.ToString());

            if (applicationUser is null)
                return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

            if (await _userManager.HasPasswordAsync(applicationUser))
            {
                var removeResult = await _userManager.RemovePasswordAsync(applicationUser);

                if (!removeResult.Succeeded)
                    return OperationResult.Failure("Failed to remove old password.", OperationErrorType.Technical);
            }

            var addResult = await _userManager.AddPasswordAsync(applicationUser, setPasswordDto.NewPassword);

            if (!addResult.Succeeded)
                return OperationResult.Failure("Failed to reset password. Ensure the password meets complexity requirements.", OperationErrorType.ValidationError);

            await _userManager.UpdateSecurityStampAsync(applicationUser);

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
                return OperationResult.Failure("Invalid user ID.", OperationErrorType.ValidationError);

            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

            var deleteResult = await _userManager.DeleteAsync(applicationUser);

            if (!deleteResult.Succeeded)
                return OperationResult.Failure("Failed to delete user.", OperationErrorType.Technical);

            return OperationResult.Success();
        }
    }
}
