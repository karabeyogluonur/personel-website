using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
   public void Configure(EntityTypeBuilder<Category> builder)
   {
      builder.ToTable("Categories");

      builder.HasKey(category => category.Id);

      builder.Property(category => category.Name)
          .IsRequired()
          .HasMaxLength(ApplicationLimits.Category.NameMaxLength);

      builder.Property(category => category.Description)
          .HasMaxLength(ApplicationLimits.Category.DescriptionMaxLength);

      builder.Property(category => category.CreatedAt)
          .IsRequired();

      builder.Property(category => category.IsDeleted)
          .IsRequired()
          .HasDefaultValue(false);

      builder.HasIndex(category => category.Name)
          .IsUnique()
          .HasFilter("\"IsDeleted\" = false");

      builder.HasMany(category => category.Translations)
          .WithOne(translation => translation.Entity)
          .HasForeignKey(translation => translation.EntityId)
          .OnDelete(DeleteBehavior.Cascade);
   }
}
