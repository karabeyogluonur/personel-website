using PW.Domain.Common;

namespace PW.Domain.Entities;

public class Language : BaseEntity, ISoftDeleteEntity, IAuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string FlagImageFileName { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
