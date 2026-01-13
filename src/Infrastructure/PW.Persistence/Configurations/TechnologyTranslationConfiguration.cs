using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class TechnologyTranslationConfiguration : IEntityTypeConfiguration<TechnologyTranslation>
{
   public void Configure(EntityTypeBuilder<TechnologyTranslation> builder)
   {
      builder.ToTable("TechnologyTranslations");

      builder.HasKey(translation => translation.Id);

      builder.Property(translation => translation.Name)
          .HasMaxLength(ApplicationLimits.Technology.NameMaxLength);

      builder.Property(translation => translation.Description)
          .HasMaxLength(ApplicationLimits.Technology.DescriptionMaxLength);

      builder.HasOne(translation => translation.Language)
          .WithMany()
          .HasForeignKey(translation => translation.LanguageId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(translation => translation.Entity)
          .WithMany(technology => technology.Translations)
          .HasForeignKey(translation => translation.EntityId);

      builder.HasIndex(translation => new { translation.EntityId, translation.LanguageId })
          .IsUnique();
   }
}
