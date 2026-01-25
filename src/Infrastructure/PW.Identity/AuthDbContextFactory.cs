using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PW.Identity.Contexts;

namespace PW.Identity;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
   public AuthDbContext CreateDbContext(string[] args)
   {
      var currentDirectory = Directory.GetCurrentDirectory();
      var webProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "Presentation", "PW.Admin.Web"));

      var config = new ConfigurationBuilder()
          .SetBasePath(webProjectPath)
          .AddJsonFile("appsettings.json", optional: true)
          .AddJsonFile("appsettings.Development.json", optional: true)
          .AddEnvironmentVariables()
          .Build();

      var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

      var connectionString = config.GetConnectionString("DefaultConnection");

      if (string.IsNullOrEmpty(connectionString))
      {
         throw new InvalidOperationException(
             "Connection string 'DefaultConnection' not found in configuration. " +
             $"Looked in path: {webProjectPath}"
         );
      }

      optionsBuilder.UseNpgsql(connectionString);

      return new AuthDbContext(optionsBuilder.Options);
   }
}
