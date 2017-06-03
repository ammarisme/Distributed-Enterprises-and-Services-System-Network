using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RetailEnterprise.Startup))]
namespace RetailEnterprise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
