using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PW.Persistence.Contexts;

namespace PW.Persistence;

public class PWDbContextFactory : IDesignTimeDbContextFactory<PWDbContext>
{
    public PWDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var webProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "Presentation", "PW.Web"));

        var config = new ConfigurationBuilder()
            .SetBasePath(webProjectPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PWDbContext>();

        var connectionString = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in configuration. " +
                $"Looked in path: {webProjectPath}"
            );
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new PWDbContext(optionsBuilder.Options);
    }
}
