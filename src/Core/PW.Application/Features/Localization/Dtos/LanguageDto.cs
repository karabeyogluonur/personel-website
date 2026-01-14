using PW.Application.Common.Dtos;

namespace PW.Application.Features.Localization.Dtos;

public class LanguageCreateDto
{
   public string Name { get; set; } = string.Empty;
   public string Code { get; set; } = string.Empty;
   public bool IsPublished { get; set; }
   public bool IsDefault { get; set; }
   public int DisplayOrder { get; set; }
   public FileUploadDto FlagImage { get; set; } = new();
}
public class LanguageUpdateDto
{
   public int LanguageId { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Code { get; set; } = string.Empty;
   public bool IsPublished { get; set; }
   public bool IsDefault { get; set; }
   public int DisplayOrder { get; set; }
   public FileUploadDto FlagImage { get; set; } = new();
}
public class LanguageSummaryDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Code { get; set; } = string.Empty;
   public string? FlagImageFileName { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set; }
   public bool IsPublished { get; set; }
   public bool IsDefault { get; set; }
   public int DisplayOrder { get; set; }
}
public class LanguageLookupDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Code { get; set; } = string.Empty;
   public string? FlagImageFileName { get; set; }
   public bool IsDefault { get; set; }
}
public class LanguageDetailDto
{
   public int Id { get; set; }
   public string Code { get; set; }
   public string Name { get; set; }
   public string FlagImageFileName { get; set; }
   public bool IsPublished { get; set; }
   public bool IsDefault { get; set; }
   public int DisplayOrder { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set; }
}
