using PW.Application.Models.Dtos.Common;

namespace PW.Application.Models.Dtos.Configurations;

public class ProfileSettingsDto
{
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string JobTitle { get; set; } = string.Empty;
   public string Biography { get; set; } = string.Empty;
   public string? AvatarFileName { get; set; }
   public string? CoverFileName { get; set; }

   public List<ProfileSettingsTranslationDto> Translations { get; set; } = new();
}

public class ProfileSettingsTranslationDto
{
   public int LanguageId { get; set; }
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string JobTitle { get; set; } = string.Empty;
   public string Biography { get; set; } = string.Empty;
   public string? AvatarFileName { get; set; }
   public string? CoverFileName { get; set; }
}

public class ProfileSettingsUpdateDto
{
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string JobTitle { get; set; } = string.Empty;
   public string Biography { get; set; } = string.Empty;
   public FileUploadDto Avatar { get; set; } = new();
   public FileUploadDto Cover { get; set; } = new();
   public List<ProfileSettingsTranslationUpdateDto> Translations { get; set; } = new();
}

public class ProfileSettingsTranslationUpdateDto
{
   public int LanguageId { get; set; }
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string JobTitle { get; set; } = string.Empty;
   public string Biography { get; set; } = string.Empty;
   public FileUploadDto Avatar { get; set; } = new();
   public FileUploadDto Cover { get; set; } = new();
}
