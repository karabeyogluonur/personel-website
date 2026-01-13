using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class SettingTranslation : BaseEntity, ITranslation<Setting>
{
    public int LanguageId { get; set; }
    public virtual Language Language { get; set; }
    public int EntityId { get; set; }
    public virtual Setting Entity { get; set; }
    public string Value { get; set; } = string.Empty;
}
