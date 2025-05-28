using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Public.Helpers
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var permissionChecker = DependencyResolver.Current.GetService<IPermissionCheckerAppService>();

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;

            try
            {
                permissionChecker?.CheckAccess(controllerName, actionName);
            }
            catch
            {
                filterContext.Result = new RedirectToRouteResult(
                    //new RouteValueDictionary(new { controller = "Auth", action = "Login" })
                    new RouteValueDictionary(new { controller = "Home", action = "Index" })
                );
            }
        }
    }

}