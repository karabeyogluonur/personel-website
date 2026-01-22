using PW.Application.Common.Interfaces;

namespace PW.Application.Features.Tags.Dtos;

public class TagTranslationDto : ITranslationDto
{
    public int LanguageId { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
public class TagCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public List<TagTranslationDto> Translations { get; set; } = new List<TagTranslationDto>();
}
public class TagUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public List<TagTranslationDto> Translations { get; set; } = new List<TagTranslationDto>();
}
public class TagSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class TagDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public List<TagTranslationDto> Translations { get; set; } = new();
}
