using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class TechnologyTranslation : BaseEntity, ITranslation<Technology>
{
   public int LanguageId { get; set; }
   public virtual Language Language { get; set; }

   public int EntityId { get; set; }
   public virtual Technology Entity { get; set; }

   public string? Name { get; set; }
   public string? Description { get; set; }
}
