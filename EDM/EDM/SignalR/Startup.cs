using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(EDM.SignalR.Startup))]

namespace EDM.SignalR {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
