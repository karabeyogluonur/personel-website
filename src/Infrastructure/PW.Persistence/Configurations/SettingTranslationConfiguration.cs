using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class SettingTranslationConfiguration : IEntityTypeConfiguration<SettingTranslation>
{
    public void Configure(EntityTypeBuilder<SettingTranslation> builder)
    {
        builder.ToTable("SettingTranslations");

        builder.HasKey(translation => translation.Id);

        builder.Property(translation => translation.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(translation => translation.Language)
            .WithMany()
            .HasForeignKey(translation => translation.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(translation => translation.Entity)
            .WithMany(setting => setting.Translations)
            .HasForeignKey(translation => translation.EntityId);

        builder.HasIndex(translation => new { translation.EntityId, translation.LanguageId })
            .IsUnique();
    }
}
