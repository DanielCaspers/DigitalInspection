using System.Threading.Tasks;

namespace Microsoft.Owin.Security.MurphyAutomotive
{
    /// <summary>
    /// Specifies callback methods which the <see cref="MurphyAutomotiveOAuth2AuthenticationMiddleware"></see> invokes to enable developer control over the authentication process. />
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
        Justification = "OAuth2 is a valid word.")]
    public interface IMurphyAutomotiveOAuth2AuthenticationProvider
    {
        /// <summary>
        /// Invoked whenever MurphyAutomotive succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task Authenticated(MurphyAutomotiveOAuth2AuthenticatedContext context);

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context">Contains context information and authentication ticket of the return endpoint.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task ReturnEndpoint(MurphyAutomotiveOAuth2ReturnEndpointContext context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the MurphyAutomotive OAuth 2.0 middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="AuthenticationProperties"/> of the challenge </param>
        void ApplyRedirect(MurphyAutomotiveOAuth2ApplyRedirectContext context);
    }
}
