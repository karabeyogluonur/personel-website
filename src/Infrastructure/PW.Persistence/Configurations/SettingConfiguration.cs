using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings");

        builder.HasKey(setting => setting.Id);

        builder.Property(setting => setting.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(setting => setting.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(setting => setting.Name)
            .IsUnique();

        builder.HasMany(setting => setting.Translations)
            .WithOne(translation => translation.Entity)
            .HasForeignKey(translation => translation.EntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
