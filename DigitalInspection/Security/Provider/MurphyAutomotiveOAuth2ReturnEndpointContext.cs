using Microsoft.Owin.Security.Provider;

namespace Microsoft.Owin.Security.MurphyAutomotive
{
    /// <summary>
    /// Provides context information to middleware providers.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
        Justification = "OAuth2 is a valid word.")]
    public class MurphyAutomotiveOAuth2ReturnEndpointContext : ReturnEndpointContext
    {
        /// <summary>
        /// Initialize a <see cref="MurphyAutomotiveOAuth2ReturnEndpointContext"/>
        /// </summary>
        /// <param name="context">OWIN environment</param>
        /// <param name="ticket">The authentication ticket</param>
        public MurphyAutomotiveOAuth2ReturnEndpointContext(
            IOwinContext context,
            AuthenticationTicket ticket)
            : base(context, ticket)
        {
        }
    }
}
