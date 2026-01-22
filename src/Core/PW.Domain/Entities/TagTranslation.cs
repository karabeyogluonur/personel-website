using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class TagTranslation : BaseEntity, ITranslation<Tag>
{
    public int LanguageId { get; set; }
    public virtual Language Language { get; set; }
    public int EntityId { get; set; }
    public virtual Tag Entity { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
