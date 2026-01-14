using Microsoft.EntityFrameworkCore;
using PW.Application.Interfaces.Identity;
using PW.Application.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PW.Identity.Contexts;
using PW.Application.Models.Dtos.Identity;
using PW.Application.Models;

namespace PW.Identity;

public class IdentityInitialiser
{
   private readonly AuthDbContext _context;
   private readonly IUserService _userService;
   private readonly IRoleService _roleService;

   public IdentityInitialiser(AuthDbContext context, IUserService userService, IRoleService roleService)
   {
      _context = context;
      _userService = userService;
      _roleService = roleService;
   }

   public async Task InitialiseAsync()
   {
      await _context.Database.MigrateAsync();
      await SeedAsync();
   }

   public async Task SeedAsync()
   {
      await SeedRolesAsync();
      await SeedUsersAsync();
   }

   private async Task SeedRolesAsync()
   {
      List<CreateRoleDto> predefinedRoles = new List<CreateRoleDto>
        {
            new CreateRoleDto { Name = ApplicationRoles.Admin, Description = "System administrator role" },
            new CreateRoleDto { Name = ApplicationRoles.Editor, Description = "Content editor role" }
        };

      foreach (CreateRoleDto roleDefinition in predefinedRoles)
         await _roleService.CreateRoleAsync(roleDefinition);
   }

   private async Task SeedUsersAsync()
   {
      var predefinedUsers = new[]
      {
            new
            {
                RoleName = ApplicationRoles.Admin,
                Email = "admin@pw.com",
                FirstName = "System",
                LastName = "Admin",
                Password = "Pass123*"
            },
            new
            {
                RoleName = ApplicationRoles.Editor,
                Email = "editor@pw.com",
                FirstName = "Content",
                LastName = "Editor",
                Password = "Pass123*"
            }
        };

      foreach (var userInfo in predefinedUsers)
      {
         UserDto? existingUser = await _userService.GetUserByEmailAsync(userInfo.Email);

         if (existingUser is null)
         {
            CreateUserDto createUserDto = new CreateUserDto
            {
               FirstName = userInfo.FirstName,
               LastName = userInfo.LastName,
               Email = userInfo.Email,
               Password = userInfo.Password,
               Roles = new List<string> { userInfo.RoleName }
            };

            OperationResult createResult = await _userService.CreateUserAsync(createUserDto);

            if (!createResult.Succeeded)
               continue;
         }
         else
         {
            bool isInRole = await _roleService.IsInRoleAsync(existingUser.Id, userInfo.RoleName);

            if (!isInRole)
            {
               UserRoleAssignmentDto userRoleAssignmentDto = new UserRoleAssignmentDto
               {
                  UserId = existingUser.Id,
                  RoleNames = new List<string> { userInfo.RoleName }
               };
               await _roleService.UpdateUserRolesAsync(userRoleAssignmentDto);
            }
         }
      }
   }
}

public static class IdentityInitialiserExtensions
{
   public static async Task InitialiseIdentityAsync(this WebApplication app)
   {
      using IServiceScope scope = app.Services.CreateScope();

      IdentityInitialiser initialiser = scope.ServiceProvider.GetRequiredService<IdentityInitialiser>();

      await initialiser.InitialiseAsync();
   }
}
