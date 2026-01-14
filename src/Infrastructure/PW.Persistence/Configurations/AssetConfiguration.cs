using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
   public void Configure(EntityTypeBuilder<Asset> builder)
   {
      builder.ToTable("Assets");

      builder.HasKey(asset => asset.Id);

      builder.Property(asset => asset.FileName)
          .HasMaxLength(255)
          .IsRequired();

      builder.HasIndex(asset => asset.FileName);

      builder.Property(asset => asset.Folder)
          .HasMaxLength(255)
          .IsRequired();

      builder.Property(asset => asset.Extension)
          .HasMaxLength(20)
          .IsRequired();

      builder.Property(asset => asset.ContentType)
          .HasMaxLength(100)
          .IsRequired();

      builder.Property(asset => asset.AltText)
          .HasMaxLength(500);

      builder.HasMany(asset => asset.Translations)
          .WithOne(translation => translation.Entity)
          .HasForeignKey(translation => translation.EntityId)
          .OnDelete(DeleteBehavior.Cascade);
   }
}
