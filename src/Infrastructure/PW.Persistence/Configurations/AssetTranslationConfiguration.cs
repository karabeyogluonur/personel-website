using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class AssetTranslationConfiguration : IEntityTypeConfiguration<AssetTranslation>
{
   public void Configure(EntityTypeBuilder<AssetTranslation> builder)
   {
      builder.ToTable("AssetTranslations");

      builder.HasKey(translation => translation.Id);

      builder.Property(translation => translation.AltText)
          .HasMaxLength(500);

      builder.HasOne(translation => translation.Language)
          .WithMany()
          .HasForeignKey(translation => translation.LanguageId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(translation => translation.Entity)
          .WithMany(asset => asset.Translations)
          .HasForeignKey(translation => translation.EntityId);

      builder.HasIndex(translation => new { translation.EntityId, translation.LanguageId })
          .IsUnique();
   }
}
