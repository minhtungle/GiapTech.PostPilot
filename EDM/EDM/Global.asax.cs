using Public.Models;
using System.Reflection;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using EDM_DB;
using EDM.App_Start;

namespace EDM {
    public class MvcApplication : System.Web.HttpApplication {
        /*Document: 
         * https://stackoverflow.com/questions/2340572/what-is-the-purpose-of-global-asax-in-asp-net
        */
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //DependencyInjectionConfig.RegisterDependencies();
        }
    }
}
