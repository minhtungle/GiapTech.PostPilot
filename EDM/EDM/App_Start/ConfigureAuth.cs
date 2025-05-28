using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace EDM.App_Start
{
    public class ConfigureAuth
    {
        public void Configure(IAppBuilder app) // Renamed method to 'Configure' to avoid conflict with class name
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(8)
            });
        }
    }
}