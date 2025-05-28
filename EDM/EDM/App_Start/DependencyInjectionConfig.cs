using EDM_DB;
using Infrastructure.Caching;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EDM.App_Start
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterDependencies()
        {
            var container = new Container();
            // ⚠️ Cấu hình mặc định cho Scoped Lifestyle
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // 👉 Đăng ký các dependency
            container.Register<EDM_DBEntities>(() => new EDM_DBEntities(), Lifestyle.Scoped);
            container.Register<IUnitOfWork, EfUnitOfWork>(Lifestyle.Scoped);

            container.Register(typeof(IRepository<,>), typeof(EfRepository<,>));
            container.Register<ICacheManager, MemoryCacheManager>();

            // ✅ Đăng ký tất cả các MVC controller
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            // ✅ Xác minh cấu hình
            container.Verify();

            // ⚙️ Gán DI resolver cho MVC
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}