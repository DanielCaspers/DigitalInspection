using System;
using Microsoft.Owin.Security.MurphyAutomotive;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="MurphyAutomotiveOAuth2AuthenticationMiddleware"/>
    /// </summary>
    public static class MurphyAutomotiveAuthenticationExtensions
    {
		/// <summary>
		/// Authenticate users using MurphyAutomotive OAuth 2.0
		/// </summary>
		/// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
		/// <param name="options">Middleware configuration options</param>
		/// <returns>The updated <see cref="IAppBuilder"/></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
            Justification = "OAuth2 is a valid word.")]
        public static IAppBuilder UseMurphyAutomotiveAuthentication(this IAppBuilder app, MurphyAutomotiveOAuth2AuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(MurphyAutomotiveOAuth2AuthenticationMiddleware), app, options);
            return app;
        }

		/// <summary>
		/// Authenticate users using MurphyAutomotive OAuth 2.0
		/// </summary>
		/// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
		/// <param name="clientId">The MurphyAutomotive assigned client id</param>
		/// <param name="clientSecret">The MurphyAutomotive assigned client secret</param>
		/// <returns>The updated <see cref="IAppBuilder"/></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
            Justification = "OAuth2 is a valid word.")]
        public static IAppBuilder UseMurphyAutomotiveAuthentication(
            this IAppBuilder app,
            string clientId,
            string clientSecret)
        {
            return UseMurphyAutomotiveAuthentication(
                app,
                new MurphyAutomotiveOAuth2AuthenticationOptions 
                { 
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });
        }
    }
}