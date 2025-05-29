using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Serivices;
using Autofac;
using Autofac.Integration.Mvc;
using EDM_DB;
using Infrastructure.Caching;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Public.AppServices;
using Public.Interfaces;
using QuanLyBaiDang.Controllers;
using System;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;

namespace EDM.App_Start
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
          
            #region ✅ Đăng ký các Controller MVC
            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterControllers(typeof(QuanLyBaiDangController).Assembly);
            #endregion

            #region Đăng ký Infrastructure
            // ✅ Đăng ký DbContext (EF Designer with EDMX)
            builder.RegisterType<EDM_DBEntities>()
                   .As<DbContext>()
                   .InstancePerRequest(); // hoặc InstancePerLifetimeScope()

            // Đăng ký UserContext để Autofac biết cách tạo IUserContext
            builder.RegisterType<UserContext>()
                   .As<IUserContext>()
                   .InstancePerRequest();

            // ✅ Đăng ký UnitOfWork
            builder.RegisterType<EfUnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerRequest();

            // ✅ Đăng ký Generic Repository
            builder.RegisterGeneric(typeof(EfRepository<,>))
                   .As(typeof(IRepository<,>))
                   .InstancePerRequest();

            // ✅ Đăng ký Cache Manager
            builder.RegisterType<MemoryCacheManager>()
                   .As<ICacheManager>()
                   .SingleInstance(); // hoặc InstancePerRequest nếu cần
            #endregion

            #region ✅ Đăng ký Application Services
            // Đăng ký PermissionCheckerAppService
            builder.RegisterType<PermissionCheckerAppService>()
                   .As<IPermissionCheckerAppService>()
                   .InstancePerRequest(); // hoặc InstancePerLifetimeScope()

            builder.RegisterType<QuanLyBaiDangAppService>()
                   .As<IQuanLyBaiDangAppService>()
                   .InstancePerRequest();
            #endregion

            #region Đăng ký IRepository
            builder.RegisterType<EfRepository<tbBaiDang, Guid>>()
                   .As<IRepository<tbBaiDang, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbNguoiDung, Guid>>()
                   .As<IRepository<tbNguoiDung, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbKieuNguoiDung, Guid>>()
                   .As<IRepository<tbKieuNguoiDung, Guid>>()
                   .InstancePerRequest();
            #endregion

            // 🔨 Build container
            var container = builder.Build();

            // ✅ Gán Autofac làm Dependency Resolver cho MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}