using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DigitalInspection.Startup))]
namespace DigitalInspection
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
