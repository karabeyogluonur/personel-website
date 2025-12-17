using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations
{
    public class TechnologyConfiguration : IEntityTypeConfiguration<Technology>
    {
        public void Configure(EntityTypeBuilder<Technology> builder)
        {
            builder.ToTable("Technologies");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(ApplicationLimits.Technology.NameMaxLength);

            builder.HasIndex(t => t.Name);

            builder.Property(t => t.IconImageFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(ApplicationLimits.Technology.DescriptionMaxLength);

            builder.Property(t => t.DocumentationUrl)
                .IsRequired()
                .HasMaxLength(ApplicationLimits.Technology.UrlMaxLength);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
