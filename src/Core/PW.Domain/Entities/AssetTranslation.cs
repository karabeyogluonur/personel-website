using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class AssetTranslation : BaseEntity, ITranslation<Asset>
{
   public int LanguageId { get; set; }
   public virtual Language Language { get; set; }
   public int EntityId { get; set; }
   public virtual Asset Entity { get; set; }
   public string? AltText { get; set; }
}
