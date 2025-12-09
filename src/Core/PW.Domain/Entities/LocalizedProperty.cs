using PW.Domain.Common;

namespace PW.Domain.Entities
{
    public class LocalizedProperty : BaseEntity
    {
        public int EntityId { get; set; }
        public int LanguageId { get; set; }
        public string LocaleKeyGroup { get; set; }
        public string LocaleKey { get; set; }
        public string LocaleValue { get; set; }
    }
}
