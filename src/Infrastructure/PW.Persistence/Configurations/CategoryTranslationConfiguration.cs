using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class CategoryTranslationConfiguration : IEntityTypeConfiguration<CategoryTranslation>
{
   public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
   {
      builder.ToTable("CategoryTranslations");

      builder.HasKey(translation => translation.Id);

      builder.Property(translation => translation.Name)
          .HasMaxLength(ApplicationLimits.Category.NameMaxLength);

      builder.Property(translation => translation.Description)
          .HasMaxLength(ApplicationLimits.Category.DescriptionMaxLength);

      builder.HasOne(translation => translation.Language)
          .WithMany()
          .HasForeignKey(translation => translation.LanguageId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(translation => translation.Entity)
          .WithMany(category => category.Translations)
          .HasForeignKey(translation => translation.EntityId);


      builder.HasIndex(translation => new { translation.EntityId, translation.LanguageId })
          .IsUnique();
   }
}
