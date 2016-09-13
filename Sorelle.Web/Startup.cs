using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sorelle.Web.Startup))]

namespace Sorelle.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}