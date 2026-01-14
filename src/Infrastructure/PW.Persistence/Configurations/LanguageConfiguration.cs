using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PW.Application.Common.Constants;
using PW.Domain.Entities;

namespace PW.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
   public void Configure(EntityTypeBuilder<Language> builder)
   {
      builder.ToTable("Languages");

      builder.HasKey(language => language.Id);

      builder.Property(language => language.Code)
          .IsRequired()
          .HasMaxLength(ApplicationLimits.Language.CodeMaxLength);

      builder.Property(language => language.Name)
          .IsRequired()
          .HasMaxLength(ApplicationLimits.Common.NameMaxLength);

      builder.Property(language => language.FlagImageFileName)
          .HasMaxLength(255);

      builder.Property(language => language.IsPublished)
          .IsRequired()
          .HasDefaultValue(true);

      builder.Property(language => language.IsDefault)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(language => language.DisplayOrder)
          .IsRequired()
          .HasDefaultValue(0);

      builder.Property(language => language.IsDeleted)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(language => language.DeletedAt)
          .IsRequired(false);

      builder.Property(language => language.CreatedAt)
          .IsRequired();

      builder.Property(language => language.UpdatedAt)
          .IsRequired(false);

      builder.HasIndex(language => language.Code)
          .IsUnique()
          .HasFilter("\"IsDeleted\" = false")
          .HasDatabaseName("IX_Language_Code_Unique_Active");
   }
}
