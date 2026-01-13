namespace PW.Application.Models.Dtos.Configurations;

public class ProfileSettingsDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? AvatarFileName { get; set; }
    public string? CoverFileName { get; set; }
    public List<ProfileSettingsTranslationDto> Translations { get; set; } = new List<ProfileSettingsTranslationDto>();
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
    public Stream? AvatarStream { get; set; }
    public string? AvatarFileName { get; set; }
    public bool RemoveAvatar { get; set; }
    public Stream? CoverStream { get; set; }
    public string? CoverFileName { get; set; }
    public bool RemoveCover { get; set; }
    public List<ProfileSettingsTranslationUpdateDto> Translations { get; set; } = new List<ProfileSettingsTranslationUpdateDto>();
}

public class ProfileSettingsTranslationUpdateDto
{
    public int LanguageId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public Stream? AvatarStream { get; set; }
    public string? AvatarFileName { get; set; }
    public bool RemoveAvatar { get; set; }
    public Stream? CoverStream { get; set; }
    public string? CoverFileName { get; set; }
    public bool RemoveCover { get; set; }
}
