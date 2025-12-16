using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .HasMaxLength(100);

            builder.HasIndex(t => t.Name);

            builder.Property(t => t.IconImageFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(t => t.DocumentationUrl)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
