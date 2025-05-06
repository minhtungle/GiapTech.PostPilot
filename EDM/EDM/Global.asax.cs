using Public.Models;
using System.Reflection;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Web.Http;
using EDM_DB;
using EDM_DB.Interfaces;

namespace EDM {
    public class MvcApplication : System.Web.HttpApplication {
        /*Document: 
         * https://stackoverflow.com/questions/2340572/what-is-the-purpose-of-global-asax-in-asp-net
        */
        protected void Application_Start() {
            RegisterDependencies();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register); // nếu có Web API
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        private void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Đăng ký Controller
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register DbContext
            builder.RegisterType<EDM_DBEntities>().AsSelf().InstancePerRequest();

            // Register Repository
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            //// Register AppService
            //builder.RegisterAssemblyTypes(typeof(PostAppService).Assembly)
            //    .Where(t => t.Name.EndsWith("AppService"))
            //    .AsImplementedInterfaces()
            //    .InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
