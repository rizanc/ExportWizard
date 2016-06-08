using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExportWizard.Startup))]
namespace ExportWizard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
