using PW.Application.Common.Interfaces;
using PW.Application.Models.Dtos.Common;

namespace PW.Application.Models.Dtos.Content;

public class TechnologySummaryDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public string? IconImageFileName { get; set; }
   public bool IsActive { get; set; }
   public DateTime CreatedAt { get; set; }
}
public class TechnologyDetailDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public bool IsActive { get; set; }
   public string? IconImageFileName { get; set; }
   public List<TechnologyTranslationDto> Translations { get; set; } = new();
}
public class TechnologyCreateDto
{
   public string Name { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public bool IsActive { get; set; }
   public FileUploadDto Icon { get; set; } = new();
   public List<TechnologyTranslationDto> Translations { get; set; } = new();
}
public class TechnologyUpdateDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public bool IsActive { get; set; }
   public FileUploadDto Icon { get; set; } = new();
   public List<TechnologyTranslationDto> Translations { get; set; } = new();
}
public class TechnologyTranslationDto : ITranslationDto
{
   public int LanguageId { get; set; }
   public string? Name { get; set; } = string.Empty;
   public string? Description { get; set; } = string.Empty;
}
