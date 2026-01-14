namespace PW.Domain.Common;

public interface ISoftDeleteEntity
{
   public bool IsDeleted { get; set; }
   public DateTime? DeletedAt { get; set; }
}
