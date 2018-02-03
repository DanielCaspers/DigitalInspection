using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Microsoft.Owin.Infrastructure;

namespace Microsoft.Owin.Security.MurphyAutomotive
{
    /// <summary>
    /// Configuration options for <see cref="MurphyAutomotiveOAuth2AuthenticationMiddleware"/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
        Justification = "OAuth2 is a valid word.")]
    public class MurphyAutomotiveOAuth2AuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Initializes a new <see cref="MurphyAutomotiveOAuth2AuthenticationOptions"/>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Microsoft.Owin.Security.MurphyAutomotive.MurphyAutomotiveOAuth2AuthenticationOptions.set_Caption(System.String)", 
            Justification = "Not localizable.")]
        public MurphyAutomotiveOAuth2AuthenticationOptions()
            : base("Murphy Automotive")
        {
            Caption = "Murphy Automotive";
	        CallbackPath = new PathString("/token"); // TODO DJC FIXME WTH IS THIS DOING
			//CallbackPath = new PathString("/signin-google"); // TODO DJC FIXME WTH IS THIS DOING
            AuthenticationMode = AuthenticationMode.Active; // TODO DJC FIXME SO said active (previously was passive)
            Scope = new List<string>();
            BackchannelTimeout = TimeSpan.FromSeconds(60);
            CookieManager = new CookieManager();

            AuthorizationEndpoint = ConfigurationManager.AppSettings.Get("MurphyAutomotiveAuthorizationUrl");
            TokenEndpoint = ConfigurationManager.AppSettings.Get("MurphyAutomotiveTokenUrl");
	        UserInformationEndpoint = "";  // Constants.UserInformationEndpoint; // TODO DJC FIXME See where needed
        }

        /// <summary>
        /// Gets or sets the MurphyAutomotive-assigned client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the MurphyAutomotive-assigned client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the URI the middleware will access to exchange the OAuth token.
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the URI the middleware will access to obtain the user information.
        /// </summary>
        public string UserInformationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the a pinned certificate validator to use to validate the endpoints used
        /// in back channel communications belong to MurphyAutomotive.
        /// </summary>
        /// <value>
        /// The pinned certificate validator.
        /// </value>
        /// <remarks>If this property is null then the default certificate checks are performed,
        /// validating the subject name and if the signing chain is a trusted party.</remarks>
        public ICertificateValidator BackchannelCertificateValidator { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with MurphyAutomotive.
        /// </summary>
        /// <value>
        /// The back channel timeout in milliseconds.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; }

        /// <summary>
        /// The HttpMessageHandler used to communicate with MurphyAutomotive.
        /// This cannot be set at the same time as BackchannelCertificateValidator unless the value 
        /// can be downcast to a WebRequestHandler.
        /// </summary>
        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        /// <summary>
        /// Get or sets the text that the user can display on a sign in user interface.
        /// </summary>
        public string Caption
        {
            get { return Description.Caption; }
            set { Description.Caption = value; }
        }

        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// Default value is "/signin-google".
        /// </summary>
        public PathString CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMurphyAutomotiveOAuth2AuthenticationProvider"/> used to handle authentication events.
        /// </summary>
        public IMurphyAutomotiveOAuth2AuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        /// <summary>
        /// A list of permissions to request.
        /// </summary>
        public IList<string> Scope { get; private set; }

        /// <summary>
        /// access_type. Set to 'offline' to request a refresh token.
        /// </summary>
        public string AccessType { get; set; }

        /// <summary>
        /// An abstraction for reading and setting cookies during the authentication process.
        /// </summary>
        public ICookieManager CookieManager { get; set; }
    }
}
