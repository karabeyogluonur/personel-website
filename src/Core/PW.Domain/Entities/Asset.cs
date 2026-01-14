using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class Asset : BaseEntity, ILocalizedEntity<AssetTranslation>
{
   public string FileName { get; set; } = string.Empty;
   public string Folder { get; set; } = string.Empty;
   public string Extension { get; set; } = string.Empty;
   public string ContentType { get; set; } = string.Empty;
   public string? AltText { get; set; }
   public virtual ICollection<AssetTranslation> Translations { get; set; } = new List<AssetTranslation>();
}
