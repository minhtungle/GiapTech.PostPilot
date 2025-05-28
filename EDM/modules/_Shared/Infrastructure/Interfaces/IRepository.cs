using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        TEntity GetById(TKey id);
        IEnumerable<TEntity> GetAll();
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IQueryable<TEntity> Query();
    }
}