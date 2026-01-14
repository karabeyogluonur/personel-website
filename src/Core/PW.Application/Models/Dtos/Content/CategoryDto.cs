using PW.Application.Common.Interfaces;

namespace PW.Application.Models.Dtos.Content;

public class CategoryTranslationDto : ITranslationDto
{
   public int LanguageId { get; set; }
   public string? Name { get; set; } = string.Empty;
   public string? Description { get; set; }
}
public class CategoryCreateDto
{
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public bool IsActive { get; set; }
   public List<CategoryTranslationDto> Translations { get; set; } = new List<CategoryTranslationDto>();
}
public class CategoryUpdateDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public bool IsActive { get; set; }
   public List<CategoryTranslationDto> Translations { get; set; } = new List<CategoryTranslationDto>();
}
public class CategorySummaryDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public bool IsActive { get; set; }
   public DateTime CreatedAt { get; set; }
}
public class CategoryDetailDto
{
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
   public bool IsActive { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set; }
   public bool IsDeleted { get; set; }
   public DateTime? DeletedAt { get; set; }
   public List<CategoryTranslationDto> Translations { get; set; } = new();
}
