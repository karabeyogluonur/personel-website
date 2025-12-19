using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Interfaces;
using PW.Domain.Common;
using PW.Domain.Entities;

namespace PW.Persistence.Contexts
{
    public class PWDbContext : DbContext, IPWDbContext
    {
        public DbSet<Language> Languages { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<LocalizedProperty> LocalizedProperties { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<Technology> Category { get; set; }

        public PWDbContext(DbContextOptions<PWDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(PWDbContext).Assembly);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var prop = Expression.Property(parameter, nameof(ISoftDeleteEntity.IsDeleted));
                    var body = Expression.Equal(prop, Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);
                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}

