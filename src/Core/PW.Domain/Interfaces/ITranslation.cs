using PW.Domain.Common;

namespace PW.Domain.Interfaces;

public interface ITranslation<TEntity> where TEntity : BaseEntity
{
    int LanguageId { get; set; }
    int EntityId { get; set; }
    TEntity Entity { get; set; }
}
