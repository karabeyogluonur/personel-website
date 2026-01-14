using PW.Domain.Common;

namespace PW.Domain.Interfaces;

public interface ITranslation<TEntity> : IEntityTranslation where TEntity : BaseEntity
{
   int EntityId { get; set; }
   TEntity Entity { get; set; }
}
