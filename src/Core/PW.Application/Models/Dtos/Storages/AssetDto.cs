namespace PW.Application.Models.Dtos.Storages;

public class AssetTranslationDto
{
    public int LanguageId { get; set; }
    public string? AltText { get; set; }
}

public class AssetUploadDto
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string? AltText { get; set; }

    public List<AssetTranslationDto> Translations { get; set; } = new List<AssetTranslationDto>();
}
