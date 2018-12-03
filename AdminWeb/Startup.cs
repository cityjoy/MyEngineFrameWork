using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdminWeb.Startup))]
namespace AdminWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
