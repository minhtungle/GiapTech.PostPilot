using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(EDM.App_Start.Startup))]
namespace EDM.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Middleware đồng bộ HttpContext.User với OWIN User
            app.Use(async (context, next) =>
            {
                var httpContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
                if (httpContext != null)
                {
                    httpContext.User = context.Authentication.User;
                }
                await next.Invoke();
            });

            // Cấu hình cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login"),
                // Thêm các cấu hình nếu cần, ví dụ:
                // ExpireTimeSpan = TimeSpan.FromHours(8),
                // SlidingExpiration = true
            });

            app.MapSignalR();
        }
    }
}