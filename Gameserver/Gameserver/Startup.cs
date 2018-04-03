using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gameserver.Startup))]
namespace Gameserver
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
