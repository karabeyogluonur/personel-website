using Microsoft.EntityFrameworkCore.Storage;
using PW.Application.Interfaces.Repositories;
using PW.Persistence.Contexts;

namespace PW.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PWDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(PWDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context);
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress. Please complete (Commit) or discard (Rollback) the current transaction.");
            }
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No active transaction found. You must invoke BeginTransactionAsync() before attempting to commit.");
            }
            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null) return;

            await _currentTransaction.RollbackAsync();

            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Rollback();
                _currentTransaction.Dispose();
            }
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
