using Microsoft.EntityFrameworkCore;
using PW.Application.Interfaces.Identity;
using PW.Application.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PW.Identity.Contexts;
using PW.Application.Models.Dtos.Identity;

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
        var predefinedRoles = new[]
        {
            new { Name = ApplicationRoles.Admin, Description = "System administrator role" },
            new { Name = ApplicationRoles.Editor, Description = "Content editor role" }
        };

        foreach (var roleDefinition in predefinedRoles)
        {
            var createRoleDto = new CreateRoleDto
            {
                Name = roleDefinition.Name,
                Description = roleDefinition.Description
            };
            await _roleService.CreateRoleAsync(createRoleDto);
        }
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
            var existingUser = await _userService.GetUserByEmailAsync(userInfo.Email);

            if (existingUser is null)
            {
                var createDto = new CreateUserDto
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Email = userInfo.Email,
                    Password = userInfo.Password,
                    Roles = new List<string> { userInfo.RoleName }
                };

                var createResult = await _userService.CreateUserAsync(createDto);

                if (!createResult.Succeeded)
                    continue;
            }

            if (existingUser is not null)
            {
                var isInRole = await _roleService.IsInRoleAsync(existingUser.Id, userInfo.RoleName);

                if (!isInRole)
                {
                    var assignmentDto = new UserRoleAssignmentDto
                    {
                        UserId = existingUser.Id,
                        RoleNames = new List<string> { userInfo.RoleName }
                    };
                    await _roleService.UpdateUserRolesAsync(assignmentDto);
                }
            }
        }
    }

}

public static class IdentityInitialiserExtensions
{
    public static async Task InitialiseIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        IdentityInitialiser initialiser;
        try
        {
            initialiser = scope.ServiceProvider.GetRequiredService<IdentityInitialiser>();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        await initialiser.InitialiseAsync();
    }
}
