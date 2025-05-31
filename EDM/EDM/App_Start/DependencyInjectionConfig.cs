using Applications._Others.AppServices;
using Applications._Others.Interfaces;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Services;
using Applications.QuanLyAITool.Interfaces;
using Applications.QuanLyAITool.Services;
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
using QuanLyAIBot.Controllers;
using QuanLyAITool.Controllers;
using QuanLyBaiDang.Controllers;
using System;
using System.Data.Entity;
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
            builder.RegisterControllers(typeof(QuanLyAIBotController).Assembly);
            builder.RegisterControllers(typeof(QuanLyAIToolController).Assembly);
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

            #region ✅ Đăng ký AppServices
            // Đăng ký PermissionCheckerAppService
            builder.RegisterType<PermissionCheckerAppService>()
                   .As<IPermissionCheckerAppService>()
                   .InstancePerRequest(); // hoặc InstancePerLifetimeScope()
            builder.RegisterType<QuanLyBaiDangAppService>()
                   .As<IQuanLyBaiDangAppService>()
                   .InstancePerRequest();
            builder.RegisterType<QuanLyAIBotAppService>()
                   .As<IQuanLyAIBotAppService>()
                   .InstancePerRequest();
            builder.RegisterType<QuanLyAIToolAppService>()
                   .As<IQuanLyAIToolAppService>()
                   .InstancePerRequest();
            builder.RegisterType<OtherAppService>()
                   .As<IOtherAppService>()
                   .InstancePerRequest();
            #endregion

            #region Đăng ký IRepositories
            builder.RegisterType<EfRepository<tbBaiDang, Guid>>()
                   .As<IRepository<tbBaiDang, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbTepDinhKem, Guid>>()
                   .As<IRepository<tbTepDinhKem, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbBaiDangTepDinhKem, Guid>>()
                   .As<IRepository<tbBaiDangTepDinhKem, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbNguoiDung, Guid>>()
                   .As<IRepository<tbNguoiDung, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbKieuNguoiDung, Guid>>()
                   .As<IRepository<tbKieuNguoiDung, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbAIBot, Guid>>()
                   .As<IRepository<tbAIBot, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbLoaiAIBot, Guid>>()
                   .As<IRepository<tbLoaiAIBot, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbAIBotLoaiAIBot, Guid>>()
                   .As<IRepository<tbAIBotLoaiAIBot, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbAITool, Guid>>()
                   .As<IRepository<tbAITool, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbNenTang, Guid>>()
                   .As<IRepository<tbNenTang, Guid>>()
                   .InstancePerRequest();
            #endregion

            // 🔨 Build container
            var container = builder.Build();

            // ✅ Gán Autofac làm Dependency Resolver cho MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}