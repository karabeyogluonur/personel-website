using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Domain.Entities;

namespace MyProject.Infrastructure.Persistence.Configurations
{
    public class LocalizedPropertyConfiguration : IEntityTypeConfiguration<LocalizedProperty>
    {
        public void Configure(EntityTypeBuilder<LocalizedProperty> builder)
        {
            builder.ToTable("LocalizedProperties");

            builder.Property(x => x.LocaleKeyGroup).HasMaxLength(400).IsRequired();
            builder.Property(x => x.LocaleKey).HasMaxLength(400).IsRequired();
            builder.Property(x => x.LocaleValue).IsRequired().HasMaxLength(int.MaxValue);
            builder.HasOne<Language>()
                .WithMany()
                .HasForeignKey(x => x.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.EntityId, x.LocaleKeyGroup });
        }
    }
}
