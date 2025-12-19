using PW.Domain.Common;
using PW.Domain.Interfaces;

namespace PW.Domain.Entities
{
    public class Category : BaseEntity, ISoftDeleteEntity, IAuditableEntity, ILocalizedEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageFileName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
