using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Domain.Entities.Localization;

namespace PW.Persistence.Configurations.Localization
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.FlagImageFileName)
                .HasMaxLength(255);

            builder.Property(l => l.IsPublished).IsRequired();
            builder.Property(l => l.IsDefault).IsRequired();
            builder.Property(l => l.DisplayOrder).IsRequired();

            builder.HasIndex(l => l.Code)
                .IsUnique()
                .HasDatabaseName("IX_Language_Code_Unique");

            builder.Property(l => l.IsDeleted).IsRequired();
            builder.Property(l => l.DeletedAt).IsRequired(false);
            builder.Property(l => l.CreatedAt).IsRequired();
            builder.Property(l => l.UpdatedAt).IsRequired(false);
        }
    }
}
