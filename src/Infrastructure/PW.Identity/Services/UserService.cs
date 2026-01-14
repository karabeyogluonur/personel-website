using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Models;
using PW.Application.Models.Dtos.Identity;
using PW.Identity.Entities;

namespace PW.Identity.Services;

public class UserService : IUserService
{
   private readonly UserManager<ApplicationUser> _userManager;

   public UserService(UserManager<ApplicationUser> userManager)
   {
      _userManager = userManager;
   }

   public async Task<OperationResult<int>> CreateUserAsync(CreateUserDto createUserDto)
   {
      if (createUserDto == null) throw new ArgumentNullException(nameof(createUserDto));

      ApplicationUser? existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
      if (existingUser != null)
         return OperationResult<int>.Failure($"Email '{createUserDto.Email}' is already registered.", OperationErrorType.Conflict);

      ApplicationUser applicationUser = new ApplicationUser
      {
         FirstName = createUserDto.FirstName,
         LastName = createUserDto.LastName,
         Email = createUserDto.Email,
         UserName = createUserDto.Email,
         EmailConfirmed = true
      };

      IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, createUserDto.Password);

      if (!identityResult.Succeeded)
      {
         string errorMessage = identityResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "User creation failed.";
         return OperationResult<int>.Failure(errorMessage, OperationErrorType.ValidationError);
      }

      if (createUserDto.Roles != null && createUserDto.Roles.Any())
      {
         IdentityResult roleResult = await _userManager.AddToRolesAsync(applicationUser, createUserDto.Roles);
         if (!roleResult.Succeeded)
            return OperationResult<int>.Failure("User created but roles could not be assigned.", OperationErrorType.Technical);
      }

      return OperationResult<int>.Success(applicationUser.Id);
   }

   public async Task<UserDto?> GetUserByIdAsync(int userId)
   {
      if (userId <= 0) return null;

      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(userId.ToString());
      if (applicationUser == null) return null;

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
      if (userId <= 0) return OperationResult.Failure("Invalid user ID.", OperationErrorType.ValidationError);

      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(userId.ToString());
      if (applicationUser == null) return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

      if (!string.Equals(applicationUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
      {
         ApplicationUser? emailCheck = await _userManager.FindByEmailAsync(userDto.Email);
         if (emailCheck != null)
            return OperationResult.Failure($"Email '{userDto.Email}' is already taken.", OperationErrorType.Conflict);

         applicationUser.Email = userDto.Email;
         applicationUser.UserName = userDto.Email;
      }

      applicationUser.FirstName = userDto.FirstName;
      applicationUser.LastName = userDto.LastName;

      IdentityResult identityResult = await _userManager.UpdateAsync(applicationUser);
      return identityResult.Succeeded
          ? OperationResult.Success()
          : OperationResult.Failure("Failed to update user.", OperationErrorType.Technical);
   }

   public async Task<OperationResult> DeleteUserAsync(int userId)
   {
      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(userId.ToString());
      if (applicationUser == null) return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

      IdentityResult deleteResult = await _userManager.DeleteAsync(applicationUser);
      return deleteResult.Succeeded
          ? OperationResult.Success()
          : OperationResult.Failure("Failed to delete user.", OperationErrorType.Technical);
   }

   public async Task<OperationResult> AdminResetUserPasswordAsync(SetPasswordDto setPasswordDto)
   {
      ApplicationUser? applicationUser = await _userManager.FindByIdAsync(setPasswordDto.UserId.ToString());
      if (applicationUser == null) return OperationResult.Failure("User not found.", OperationErrorType.NotFound);

      if (await _userManager.HasPasswordAsync(applicationUser))
      {
         await _userManager.RemovePasswordAsync(applicationUser);
      }

      IdentityResult addResult = await _userManager.AddPasswordAsync(applicationUser, setPasswordDto.NewPassword);
      if (!addResult.Succeeded)
         return OperationResult.Failure("Password reset failed.", OperationErrorType.ValidationError);

      await _userManager.UpdateSecurityStampAsync(applicationUser);
      return OperationResult.Success();
   }

   public async Task<UserDto?> GetUserByEmailAsync(string email)
   {
      if (string.IsNullOrWhiteSpace(email)) return null;

      ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);
      if (applicationUser == null) return null;

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
}
