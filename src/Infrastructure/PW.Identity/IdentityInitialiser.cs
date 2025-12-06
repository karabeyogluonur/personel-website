using Microsoft.EntityFrameworkCore;
using PW.Application.Interfaces.Identity;
using PW.Application.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PW.Identity.Contexts;

namespace PW.Identity
{
    public class IdentityInitialiser
    {
        private readonly AuthDbContext _context;
        private readonly IIdentityService _identityService;

        public IdentityInitialiser(AuthDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
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
                new { Name = ApplicationRoles.Admin,  Description = "System administrator role" },
                new { Name = ApplicationRoles.Editor, Description = "Content editor role" }
            };

            foreach (var roleDefinition in predefinedRoles)
            {
                await _identityService.CreateRoleAsync(
                    roleDefinition.Name,
                    roleDefinition.Description
                );
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
                var existingUserId = await _identityService.FindByEmailAsync(userInfo.Email);

                if (existingUserId is null)
                {
                    var createResult = await _identityService.CreateUserAsync(
                        userInfo.FirstName,
                        userInfo.LastName,
                        userInfo.Email,
                        userInfo.Password
                    );

                    if (!createResult.Succeeded)
                        continue;

                    existingUserId = createResult.Data;
                }
                await _identityService.AssignRoleAsync(existingUserId.Value, userInfo.RoleName);
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
}
