using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Engine.Web.Startup))]
namespace Engine.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           
        }
    }
}
