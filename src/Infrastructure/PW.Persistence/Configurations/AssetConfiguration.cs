using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable("Assets");

            builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            builder.HasIndex(x => x.FileName);

            builder.Property(x => x.Folder).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Extension).HasMaxLength(20).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();

            builder.Property(x => x.AltText).HasMaxLength(500);
        }
    }
}
