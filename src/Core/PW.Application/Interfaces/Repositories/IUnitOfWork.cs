namespace PW.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{

   Task<int> CommitAsync();

   IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

   Task BeginTransactionAsync();

   Task CommitTransactionAsync();

   Task RollbackTransactionAsync();
}
