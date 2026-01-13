using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class TechnologyConfiguration : IEntityTypeConfiguration<Technology>
{
   public void Configure(EntityTypeBuilder<Technology> builder)
   {
      builder.ToTable("Technologies");

      builder.HasKey(technology => technology.Id);

      builder.Property(technology => technology.Name)
          .IsRequired()
          .HasMaxLength(ApplicationLimits.Technology.NameMaxLength);

      builder.HasIndex(technology => technology.Name);

      builder.Property(technology => technology.IconImageFileName)
          .HasMaxLength(255)
          .IsRequired();

      builder.Property(technology => technology.Description)
          .IsRequired()
          .HasMaxLength(ApplicationLimits.Technology.DescriptionMaxLength);

      builder.Property(technology => technology.IsActive)
          .HasDefaultValue(true)
          .IsRequired();

      builder.HasMany(technology => technology.Translations)
          .WithOne(translation => translation.Entity)
          .HasForeignKey(translation => translation.EntityId)
          .OnDelete(DeleteBehavior.Cascade);
   }
}
