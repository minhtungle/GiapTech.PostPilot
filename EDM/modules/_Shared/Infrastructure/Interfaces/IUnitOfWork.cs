using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class;

        // Save changes
        int SaveChanges();
        Task<int> SaveChangesAsync();

        // Async CRUD helpers
        Task InsertAsync<TEntity, TKey>(TEntity entity) where TEntity : class;
        Task UpdateAsync<TEntity, TKey>(TEntity entity) where TEntity : class;
        Task DeleteAsync<TEntity, TKey>(TEntity entity) where TEntity : class;

        // Transaction
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task ExecuteInTransaction(Func<Task> action);
    }
}
