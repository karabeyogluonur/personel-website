using System.Reflection;
using Microsoft.EntityFrameworkCore;
namespace PW.Persistence.Contexts
{
    public class PWDbContext : DbContext
    {
        public PWDbContext(DbContextOptions<PWDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

