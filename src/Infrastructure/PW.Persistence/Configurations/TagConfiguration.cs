using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Name)
            .IsRequired()
            .HasMaxLength(ApplicationLimits.Tag.NameMaxLength);

        builder.Property(tag => tag.Description)
            .HasMaxLength(ApplicationLimits.Tag.DescriptionMaxLength);

        builder.Property(tag => tag.CreatedAt)
            .IsRequired();

        builder.Property(tag => tag.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(tag => tag.Name)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasMany(tag => tag.Translations)
            .WithOne(translation => translation.Entity)
            .HasForeignKey(translation => translation.EntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
