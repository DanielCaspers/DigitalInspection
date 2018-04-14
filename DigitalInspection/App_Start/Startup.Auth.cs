using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using DigitalInspection.Models;
using System.Configuration;
using DigitalInspection.Services.Web;

namespace DigitalInspection
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Configure the db context, user manager and signin manager to use a single instance per request
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

			var authValidationTimeInMinutes = Double.Parse(ConfigurationManager.AppSettings.Get("MurphyAutomotiveAuthValidationIntervalInMinutes"));
			var authValidationInterval = TimeSpan.FromMinutes(authValidationTimeInMinutes);

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
				ExpireTimeSpan = authValidationInterval,

				// Under the current use, we cannot allow a sliding expiration. This must be in lock step with the validateInterval
				SlidingExpiration = false,
				Provider = new CookieAuthenticationProvider
				{
					// Enables the application to validate the security stamp when the user logs in.
					// This is a security feature which is used when you change a password or add an external login to your account.  
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
						validateInterval: authValidationInterval,
						regenerateIdentity: (manager, user) => AuthenticationService.RefreshIdentityAsync(manager, user))
				}
			});
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
		}
	}
}
