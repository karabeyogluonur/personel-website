using PW.Application.Common.Enums;
using PW.Application.Interfaces.Identity;
using PW.Application.Interfaces.Identity.Dtos;
using PW.Application.Interfaces.Repositories;
using PW.Application.Utilities.Results;
using static PW.Application.Features.Users.Dtos.UserDto;

namespace PW.Application.Features.Users;

public class UserService : IUserService
{
   private readonly IIdentityUserService _identityUserService;
   private readonly IIdentityRoleService _identityRoleService;
   private readonly IUnitOfWork _unitOfWork;

   public UserService(
       IIdentityUserService identityUserService,
       IIdentityRoleService identityRoleService,
       IUnitOfWork unitOfWork)
   {
      _identityUserService = identityUserService;
      _identityRoleService = identityRoleService;
      _unitOfWork = unitOfWork;
   }

   public async Task<IList<UserSummaryDto>> GetAllUsersAsync()
   {
      IList<IdentityUserDto> identityUserDtos = await _identityUserService.GetAllUsersAsync();

      IList<UserSummaryDto> userSummaryDtos = identityUserDtos.Select((IdentityUserDto identityUserDto) => new UserSummaryDto
      {
         Id = identityUserDto.Id,
         FirstName = identityUserDto.FirstName,
         LastName = identityUserDto.LastName,
         Email = identityUserDto.Email,
         Roles = identityUserDto.Roles
      }).ToList();

      return userSummaryDtos;
   }

   public async Task<UserDetailDto?> GetUserByIdAsync(int userId)
   {
      IdentityUserDto? identityUserDto = await _identityUserService.GetUserByIdAsync(userId);

      if (identityUserDto == null)
         return null;

      UserDetailDto userDetailDto = new UserDetailDto
      {
         Id = identityUserDto.Id,
         FirstName = identityUserDto.FirstName,
         LastName = identityUserDto.LastName,
         Email = identityUserDto.Email,
         Roles = identityUserDto.Roles
      };

      return userDetailDto;
   }

   public async Task<IList<string>> GetAllRolesAsync()
   {
      return await _identityRoleService.GetAllRolesAsync();
   }

   public async Task<OperationResult> CreateUserAsync(UserCreateDto userCreateDto)
   {
      IdentityCreateUserDto identityCreateUserDto = new IdentityCreateUserDto
      {
         FirstName = userCreateDto.FirstName,
         LastName = userCreateDto.LastName,
         Email = userCreateDto.Email,
         Password = userCreateDto.Password,
         Roles = userCreateDto.Roles
      };

      return await _identityUserService.CreateUserAsync(identityCreateUserDto);
   }

   public async Task<OperationResult> UpdateUserAsync(UserUpdateDto userUpdateDto)
   {
      await _unitOfWork.BeginTransactionAsync();

      try
      {
         IdentityUserDto identityUserDto = new IdentityUserDto
         {
            Id = userUpdateDto.Id,
            FirstName = userUpdateDto.FirstName,
            LastName = userUpdateDto.LastName,
            Email = userUpdateDto.Email
         };

         OperationResult updateUserResult = await _identityUserService.UpdateUserAsync(userUpdateDto.Id, identityUserDto);

         if (!updateUserResult.Succeeded)
         {
            await _unitOfWork.RollbackTransactionAsync();
            return updateUserResult;
         }

         IdentityUserRoleAssignmentDto identityUserRoleAssignmentDto = new IdentityUserRoleAssignmentDto
         {
            UserId = userUpdateDto.Id,
            RoleNames = userUpdateDto.SelectedRoles
         };

         OperationResult updateRoleResult = await _identityRoleService.UpdateUserRolesAsync(identityUserRoleAssignmentDto);

         if (!updateRoleResult.Succeeded)
         {
            await _unitOfWork.RollbackTransactionAsync();
            return updateRoleResult;
         }

         if (userUpdateDto.IsPasswordChangeRequested && !string.IsNullOrWhiteSpace(userUpdateDto.NewPassword))
         {
            IdentitySetPasswordDto identitySetPasswordDto = new IdentitySetPasswordDto
            {
               UserId = userUpdateDto.Id,
               NewPassword = userUpdateDto.NewPassword
            };

            OperationResult setPasswordResult = await _identityUserService.AdminResetUserPasswordAsync(identitySetPasswordDto);

            if (!setPasswordResult.Succeeded)
            {
               await _unitOfWork.RollbackTransactionAsync();
               return setPasswordResult;
            }
         }

         await _unitOfWork.CommitTransactionAsync();
         return OperationResult.Success();
      }
      catch (Exception exception)
      {
         await _unitOfWork.RollbackTransactionAsync();
         return OperationResult.Failure($"System error: {exception.Message}", OperationErrorType.Technical);
      }
   }

   public async Task<OperationResult> DeleteUserAsync(int userId)
   {
      if (userId <= 0)
         return OperationResult.Failure("Invalid Id", OperationErrorType.ValidationError);

      return await _identityUserService.DeleteUserAsync(userId);
   }
}
