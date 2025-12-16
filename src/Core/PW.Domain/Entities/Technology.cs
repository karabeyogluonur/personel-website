using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities
{
    public class Technology : BaseEntity, IAuditableEntity, ILocalizedEntity
    {
        public string Name { get; set; } = string.Empty;
        public string IconImageFileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string DocumentationUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
