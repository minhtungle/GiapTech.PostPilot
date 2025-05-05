using Public.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
        }
        protected void Application_End() {
        }
        protected void Session_Start() {

        }
        protected void Session_End() {
        }
        protected void Application_BeginRequest() {

        }
        protected void Application_AuthenticateRequest() {

        }
        protected void Application_Error() {
            // Khi xảy ra lỗi sẽ gọi Error vào Layout đang sử dụng
            //return View("~/Views/_Shared/Error/Index.cshtml");
        }
    }
}
