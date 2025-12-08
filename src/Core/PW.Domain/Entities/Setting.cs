using PW.Domain.Common;

namespace PW.Domain.Entities
{
    public class Setting : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
    }
}
