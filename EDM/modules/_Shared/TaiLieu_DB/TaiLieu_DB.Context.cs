﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaiLieu_DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class tailieu_dbEntities : DbContext
    {
        public tailieu_dbEntities()
            : base("name=tailieu_dbEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbLoaiTaiLieu> tbLoaiTaiLieux { get; set; }
        public virtual DbSet<tbTaiLieu> tbTaiLieux { get; set; }
    }
}
