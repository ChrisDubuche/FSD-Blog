using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSD_Blog.Startup))]
namespace FSD_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
