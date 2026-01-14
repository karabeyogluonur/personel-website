using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using PW.Application.Common.Interfaces;
using PW.Domain.Common;
using PW.Domain.Entities;

namespace PW.Persistence.Contexts;

public class PWDbContext : DbContext, IPWDbContext
{
   public PWDbContext(DbContextOptions<PWDbContext> options) : base(options)
   {
   }

   public DbSet<Language> Languages { get; set; }
   public DbSet<Setting> Settings { get; set; }
   public DbSet<SettingTranslation> SettingTranslations { get; set; }
   public DbSet<Asset> Assets { get; set; }
   public DbSet<AssetTranslation> AssetTranslations { get; set; }
   public DbSet<Technology> Technologies { get; set; }
   public DbSet<TechnologyTranslation> TechnologyTranslations { get; set; }
   public DbSet<Category> Categories { get; set; }
   public DbSet<CategoryTranslation> CategoryTranslations { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfigurationsFromAssembly(typeof(PWDbContext).Assembly);

      foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
      {
         if (typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
         {
            ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "e");
            MemberExpression prop = Expression.Property(parameter, nameof(ISoftDeleteEntity.IsDeleted));
            BinaryExpression body = Expression.Equal(prop, Expression.Constant(false));
            LambdaExpression lambda = Expression.Lambda(body, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
         }
      }
   }
}
