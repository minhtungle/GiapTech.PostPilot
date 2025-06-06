using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Repositories
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IUserContext _userContext;

        public EfRepository(DbContext context, IUserContext userContext)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _userContext = userContext;
        }

        //public virtual IQueryable<TEntity> Query()
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    if (typeof(ISoftDeleteAndTenant).IsAssignableFrom(typeof(TEntity)))
        //    {
        //        var currentDonViId = _userContext.DonViId;

        //        // Filter tự động với entity có 2 trường TrangThai và MaDonViSuDung
        //        query = query.Where(e =>
        //            ((ISoftDeleteAndTenant)e).TrangThai != 0 &&
        //            ((ISoftDeleteAndTenant)e).MaDonViSuDung == currentDonViId);
        //    }

        //    return query;
        //}

        //public virtual IQueryable<TEntity> Query()
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    var currentDonViId = _userContext.DonViId;

        //    // Filter tự động với entity có 2 trường TrangThai và MaDonViSuDung
        //    query = query.Where(e =>
        //        ((ISoftDeleteAndTenant)e).TrangThai != 0 &&
        //        ((ISoftDeleteAndTenant)e).MaDonViSuDung == currentDonViId);

        //    return query;
        //}
        public virtual IQueryable<TEntity> _Query()
        {
            IQueryable<TEntity> query = _dbSet;

            var entityType = typeof(TEntity);
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            var trangThaiProp = typeof(TEntity).GetProperty("TrangThai");
            var maDonViProp = typeof(TEntity).GetProperty("MaDonViSuDung");

            if (trangThaiProp != null && maDonViProp != null)
            {
                var trangThaiAccess = Expression.Property(parameter, trangThaiProp);
                var maDonViAccess = Expression.Property(parameter, maDonViProp);

                // TrangThai != null && TrangThai != 0
                var trangThaiNotNull = Expression.NotEqual(trangThaiAccess, Expression.Constant(null, trangThaiProp.PropertyType));
                var trangThaiNotZero = Expression.NotEqual(trangThaiAccess, Expression.Constant(0, trangThaiProp.PropertyType));
                var trangThaiCondition = Expression.AndAlso(trangThaiNotNull, trangThaiNotZero);

                // MaDonViSuDung != null
                var maDonViNotNull = Expression.NotEqual(maDonViAccess, Expression.Constant(null, maDonViProp.PropertyType));

                // MaDonViSuDung.Value == currentDonViId
                var maDonViValue = Expression.Property(maDonViAccess, "Value");
                var currentDonViIdConst = Expression.Constant(_userContext.DonViId, typeof(Guid));
                var maDonViEquals = Expression.Equal(maDonViValue, currentDonViIdConst);

                // Kết hợp điều kiện MaDonViSuDung
                var maDonViCondition = Expression.AndAlso(maDonViNotNull, maDonViEquals);

                // Kết hợp tổng thể
                var totalCondition = Expression.AndAlso(trangThaiCondition, maDonViCondition);

                var lambda = Expression.Lambda<Func<TEntity, bool>>(totalCondition, parameter);

                return _dbSet.Where(lambda);
            }

            return _dbSet;
        }
        public IQueryable<TEntity> QueryByTenant<TEntity>(bool isGetAll = true) where TEntity : class
        {
            if (isGetAll) return _context.Set<TEntity>();

            var parameter = Expression.Parameter(typeof(TEntity), "e");

            var trangThaiProp = typeof(TEntity).GetProperty("TrangThai");
            var maDonViProp = typeof(TEntity).GetProperty("MaDonViSuDung");

            if (trangThaiProp != null && maDonViProp != null)
            {
                var trangThai = Expression.NotEqual(
                    Expression.Property(parameter, "TrangThai"),
                    Expression.Constant(0));

                var maDonVi = Expression.Equal(
                    Expression.Property(parameter, "MaDonViSuDung"),
                    Expression.Constant(_userContext.DonViSuDung.MaDonViSuDung));

                var andExp = Expression.AndAlso(trangThai, maDonVi);

                var lambda = Expression.Lambda<Func<TEntity, bool>>(andExp, parameter);

                return _context.Set<TEntity>().Where(lambda);
            }

            return _context.Set<TEntity>();
        }


        public async Task<List<TEntity>> GetAllByTenantAsync<TEntity>() where TEntity : class
        {
            return await QueryByTenant<TEntity>().ToListAsync();
        }

        // Sync methods
        public TEntity GetById(TKey id) => _dbSet.Find(id);

        public IEnumerable<TEntity> GetAll() => _dbSet.ToList();

        public IQueryable<TEntity> Query() => _dbSet.AsQueryable();

        public void Add(TEntity entity) => _dbSet.Add(entity);

        public void Update(TEntity entity) => _context.Entry(entity).State = EntityState.Modified;

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        // Async methods
        public async Task<TEntity> GetByIdAsync(TKey id) => await _dbSet.FindAsync(id);

        public async Task<List<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<List<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();
    }

}