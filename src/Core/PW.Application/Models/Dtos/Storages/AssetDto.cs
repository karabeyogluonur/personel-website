using PW.Application.Common.Interfaces;
using PW.Application.Models.Dtos.Common;

namespace PW.Application.Models.Dtos.Storages;

public class AssetTranslationDto : ITranslationDto
{
   public int LanguageId { get; set; }
   public string? AltText { get; set; }
}

public class AssetUploadDto
{
   public FileUploadDto File { get; set; } = new();
   public string ContentType { get; set; } = string.Empty;
   public string Folder { get; set; } = string.Empty;
   public string SeoTitle { get; set; } = string.Empty;
   public string? AltText { get; set; }

   public List<AssetTranslationDto> Translations { get; set; } = new List<AssetTranslationDto>();
}
