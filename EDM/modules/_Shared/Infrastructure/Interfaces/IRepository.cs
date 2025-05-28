using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        // Synchronous
        TEntity GetById(TKey id);
        IEnumerable<TEntity> GetAll();
        IQueryable<TEntity> Query();
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        // Asynchronous
        Task<TEntity> GetByIdAsync(TKey id);
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> predicate);
    }
}