using AuthorizationCodeFlow;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace AuthorizationCodeFlow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app) { }
    }
}
