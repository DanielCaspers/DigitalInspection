using System;
using System.Threading.Tasks;

namespace Microsoft.Owin.Security.MurphyAutomotive
{
        /// <summary>
    /// Default <see cref="IMurphyAutomotiveOAuth2AuthenticationProvider"/> implementation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
        Justification = "OAuth2 is a valid word.")]
    public class MurphyAutomotiveOAuth2AuthenticationProvider : IMurphyAutomotiveOAuth2AuthenticationProvider
    {
        /// <summary>
        /// Initializes a <see cref="MurphyAutomotiveOAuth2AuthenticationProvider"/>
        /// </summary>
        public MurphyAutomotiveOAuth2AuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
            OnApplyRedirect = context =>
                context.Response.Redirect(context.RedirectUri);
        }

        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<MurphyAutomotiveOAuth2AuthenticatedContext, Task> OnAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the function that is invoked when the ReturnEndpoint method is invoked.
        /// </summary>
        public Func<MurphyAutomotiveOAuth2ReturnEndpointContext, Task> OnReturnEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is invoked when the ApplyRedirect method is invoked.
        /// </summary>
        public Action<MurphyAutomotiveOAuth2ApplyRedirectContext> OnApplyRedirect { get; set; }

        /// <summary>
        /// Invoked whenever MurphyAutomotive succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task Authenticated(MurphyAutomotiveOAuth2AuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context">Contains context information and authentication ticket of the return endpoint.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task ReturnEndpoint(MurphyAutomotiveOAuth2ReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the MurphyAutomotive OAuth 2.0 middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="AuthenticationProperties"/> of the challenge </param>
        public virtual void ApplyRedirect(MurphyAutomotiveOAuth2ApplyRedirectContext context)
        {
            OnApplyRedirect(context);
        }
    }
}
