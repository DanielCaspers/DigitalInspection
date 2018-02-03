using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.Security.MurphyAutomotive
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
        Justification = "OAuth2 is a valid word.")]
    public class MurphyAutomotiveOAuth2AuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="MurphyAutomotiveOAuth2AuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized MurphyAutomotive user info</param>
        /// <param name="accessToken">MurphyAutomotive OAuth 2.0 access token</param>
        /// <param name="refreshToken">Goolge OAuth 2.0 refresh token</param>
        /// <param name="expires">Seconds until expiration</param>
        public MurphyAutomotiveOAuth2AuthenticatedContext(IOwinContext context, JObject user, string accessToken, 
            string refreshToken, string expires)
            : base(context)
        {
            User = user;
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

	        UserId = TryGetValue(user, "userID");
	        FirstName = TryGetValue(user, "firstName");
	        LastName = TryGetValue(user, "lastName");
	        EmployeeId = TryGetValue(user, "empID");
	        UserImage = TryGetValue(user, "userImage");
	        //Companies = TryGetFirstValue(user, "conos");
	        //Roles = TryGetValue(user, "roles");
		}

		/// <summary>
		/// Initializes a <see cref="MurphyAutomotiveOAuth2AuthenticatedContext"/>
		/// </summary>
		/// <param name="context">The OWIN environment</param>
		/// <param name="user">The JSON-serialized MurphyAutomotive user info</param>
		/// <param name="tokenResponse">The JSON-serialized token response MurphyAutomotive</param>
		public MurphyAutomotiveOAuth2AuthenticatedContext(IOwinContext context, JObject user, JObject tokenResponse)
            : base(context)
        {
            User = user;
            TokenResponse = tokenResponse;
            if (tokenResponse != null)
            {
                AccessToken = tokenResponse.Value<string>("access_token");
                RefreshToken = tokenResponse.Value<string>("refresh_token");

                int expiresValue;
                if (Int32.TryParse(tokenResponse.Value<string>("expires_in"), NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
                {
                    ExpiresIn = TimeSpan.FromSeconds(expiresValue);
                }
            }
			// TODO: DJC FIXME AND PARSE OTHER TOKEN VALUES OUT HERE USING CLASS PROPERTIES BELOW

	        UserId = TryGetValue(user, "userID");
	        FirstName = TryGetValue(user, "firstName");
	        LastName = TryGetValue(user, "lastName");
	        EmployeeId = TryGetValue(user, "empID");
	        UserImage = TryGetValue(user, "userImage");
	        //Companies = TryGetFirstValue(user, "conos");
	        //Roles = TryGetValue(user, "roles");
		}

		/// <summary>
		/// Gets the JSON-serialized user
		/// </summary>
		/// <remarks>
		/// Contains the MurphyAutomotive user obtained from the endpoint https://www.googleapis.com/oauth2/v3/userinfo
		/// </remarks>
		// TODO DJC FIXME Do we need to have a userinfo endpoint for this?
		public JObject User { get; private set; }

        /// <summary>
        /// Gets the MurphyAutomotive access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the MurphyAutomotive refresh token
        /// </summary>
        /// <remarks>
        /// This value is not null only when access_type authorize parameter is offline.
        /// </remarks>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the MurphyAutomotive access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the MurphyAutomotive user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the user's first name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the user's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the user's employee Id
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets the user's profile link
        /// </summary>
        public string Profile { get; private set; }

        /// <summary>
        /// Gets the user's image
        /// </summary>
        public string UserImage { get; private set; }

		public IList<string> Companies { get; set; }

		// TODO: Must fit authorizationRoles into this
	    public IList<string> Roles { get; set; }

	    /// <summary>
		/// Gets the <see cref="ClaimsIdentity"/> representing the user
		/// </summary>
		public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Token response from MurphyAutomotive
        /// </summary>
        public JObject TokenResponse { get; private set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }

        // Get the given subProperty from a property.
        private static string TryGetValue(JObject user, string propertyName, string subProperty)
        {
            JToken value;
            if (user.TryGetValue(propertyName, out value))
            {
                var subObject = JObject.Parse(value.ToString());
                if (subObject != null && subObject.TryGetValue(subProperty, out value))
                {
                    return value.ToString();
                }
            }
            return null;
        }

        // Get the given subProperty from a list property.
        private static string TryGetFirstValue(JObject user, string propertyName, string subProperty)
        {
            JToken value;
            if (user.TryGetValue(propertyName, out value))
            {
                var array = JArray.Parse(value.ToString());
                if (array != null && array.Count > 0)
                {
                    var subObject = JObject.Parse(array.First.ToString());
                    if (subObject != null)
                    {
                        if (subObject.TryGetValue(subProperty, out value))
                        {
                            return value.ToString();
                        }
                    }
                }
            }
            return null;
        }
    }
}
