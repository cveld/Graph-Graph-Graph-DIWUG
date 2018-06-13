using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EventManagerDemo.Startup))]

namespace EventManagerDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}