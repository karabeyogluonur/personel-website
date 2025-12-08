using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PW.Application.Common.Models;
using PW.Application.Interfaces.Identity;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;
using PW.Identity.Extensions;
using System.Linq;

namespace PW.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<OperationResult<int>> CreateUserAsync(CreateUserDto createUserDto)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);

            if (existingUser != null)
                return OperationResult<int>.Failure($"Email '{createUserDto.Email}' is already registered.");

            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                UserName = createUserDto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(applicationUser, createUserDto.Password);

            if (!result.Succeeded)
                return result.ToOperationResult<int>();

            if (createUserDto.Roles != null && createUserDto.Roles.Any())
                await _userManager.AddToRolesAsync(applicationUser, createUserDto.Roles);

            return OperationResult<int>.Success(applicationUser.Id);
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null) return null;

            IList<string> roles = await _userManager.GetRolesAsync(applicationUser);

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
            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser is null) return null;

            IList<string> roles = await _userManager.GetRolesAsync(applicationUser);

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
            //TODO: N+1 and performance issues will be resolved.

            List<ApplicationUser> applicationUsers = await _userManager.Users.AsNoTracking().ToListAsync();

            List<UserDto> userDtos = new List<UserDto>();

            foreach (ApplicationUser applicationUser in applicationUsers)
            {
                IList<string> userRoleNames = await _userManager.GetRolesAsync(applicationUser);

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
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId.ToString());
            if (applicationUser == null) return OperationResult.Failure("User not found.");

            applicationUser.FirstName = userDto.FirstName;
            applicationUser.LastName = userDto.LastName;

            IdentityResult result = await _userManager.UpdateAsync(applicationUser);
            return result.ToOperationResult();
        }

        public async Task<OperationResult> AdminResetUserPasswordAsync(SetPasswordDto setPasswordDto)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(setPasswordDto.UserId.ToString());
            if (applicationUser == null)
                return OperationResult.Failure("User not found.");

            if (await _userManager.HasPasswordAsync(applicationUser))
                await _userManager.RemovePasswordAsync(applicationUser);

            IdentityResult addResult = await _userManager.AddPasswordAsync(applicationUser, setPasswordDto.NewPassword);

            if (!addResult.Succeeded)
                return addResult.ToOperationResult();

            await _userManager.UpdateSecurityStampAsync(applicationUser);

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteUserAsync(int userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null) return OperationResult.Failure("User not found.");

            IdentityResult deleteResult = await _userManager.DeleteAsync(applicationUser);
            return deleteResult.ToOperationResult();
        }
    }
}
