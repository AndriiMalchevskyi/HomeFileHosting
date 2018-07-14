using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeFileHosting.Startup))]
namespace HomeFileHosting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
