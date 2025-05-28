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

        int SaveChanges();               // Sync
        Task<int> SaveChangesAsync();   // Async
    }

}