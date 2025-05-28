using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Infrastructure.Repositories
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public TEntity GetById(TKey id) => _dbSet.Find(id);
        public IEnumerable<TEntity> GetAll() => _dbSet.ToList();
        public void Add(TEntity entity) => _dbSet.Add(entity);
        public void Update(TEntity entity) => _context.Entry(entity).State = EntityState.Modified;
        public void Delete(TEntity entity) => _dbSet.Remove(entity);
        public IQueryable<TEntity> Query() => _dbSet.AsQueryable();
    }
}