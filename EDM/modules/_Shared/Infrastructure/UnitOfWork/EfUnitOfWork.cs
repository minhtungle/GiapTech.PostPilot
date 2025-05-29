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

        private DbContextTransaction _transaction;

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

        public async Task InsertAsync<TEntity, TKey>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync<TEntity, TKey>(TEntity entity) where TEntity : class
        {
            _context.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task DeleteAsync<TEntity, TKey>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = _context.Database.BeginTransaction();
            }
            await Task.CompletedTask;
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _transaction?.Commit();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RollbackAsync()
        {
            _transaction?.Rollback();
            DisposeTransaction();
            await Task.CompletedTask;
        }
        public async Task ExecuteInTransaction(Func<Task> action)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await action();               // Thực thi logic nghiệp vụ async
                    _context.SaveChanges();       // Gọi SaveChanges đồng bộ (EF6 không hỗ trợ async trong transaction)
                    transaction.Commit();         // Commit transaction
                }
                catch
                {
                    transaction.Rollback();       // Rollback nếu có lỗi
                    throw;
                }
            }
        }
        private void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            DisposeTransaction();
            _context.Dispose();
        }
    }

}