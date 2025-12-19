using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ApplicationLimits.Category.NameMaxLength);

            builder.Property(x => x.Description)
                .HasMaxLength(ApplicationLimits.Category.DescriptionMaxLength);

            builder.Property(x => x.CoverImageFileName)
                .HasMaxLength(255);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");
        }
    }

}
