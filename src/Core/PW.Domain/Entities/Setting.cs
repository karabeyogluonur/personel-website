using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities;

public class Setting : BaseEntity, ILocalizedEntity<SettingTranslation>
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public virtual ICollection<SettingTranslation> Translations { get; set; } = new List<SettingTranslation>();
}
