using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities
{
    public class Setting : BaseEntity, ILocalizedEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
    }
}
