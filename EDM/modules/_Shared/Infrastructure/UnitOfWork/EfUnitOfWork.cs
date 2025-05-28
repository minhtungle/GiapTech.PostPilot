using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IUserContext _userContext;
        private readonly Dictionary<string, object> _repositories;

        public EfUnitOfWork(DbContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
            _repositories = new Dictionary<string, object>();
        }

        public IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            if (!_repositories.ContainsKey(key))
            {
                var repoInstance = new EfRepository<TEntity, TKey>(_context, _userContext);
                _repositories.Add(key, repoInstance);
            }
            return (IRepository<TEntity, TKey>)_repositories[key];
        }

        public int SaveChanges() => _context.SaveChanges();

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}