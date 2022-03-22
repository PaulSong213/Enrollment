using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EnrollmentSystem.App_Start.StartUp))]
namespace EnrollmentSystem.App_Start
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}