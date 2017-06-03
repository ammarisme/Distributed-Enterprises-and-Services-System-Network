using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WholesaleEnterprise.Startup))]
namespace WholesaleEnterprise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
