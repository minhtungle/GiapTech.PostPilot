using EDM_DB;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Infrastructure.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly EDM_DBEntities _db;

        public EfUnitOfWork(EDM_DBEntities db)
        {
            _db = db;
        }

        public int SaveChanges() => _db.SaveChanges();
    }
}