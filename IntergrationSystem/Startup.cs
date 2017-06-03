using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntegrationSystem.Startup))]
namespace IntegrationSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
