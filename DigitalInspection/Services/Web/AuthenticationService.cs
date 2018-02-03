using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using DigitalInspection.Models.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models;
using DigitalInspection.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SecurityToken = Microsoft.IdentityModel.Tokens.SecurityToken;

namespace DigitalInspection.Services
{
	public class AuthenticationService : HttpClientService
	{
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response
		public static async Task<AuthenticationResponse> Login(string username, string password)
		{
			using (HttpClient httpClient = InitializeHttpClient(false))
			{
				var postBody = new List<KeyValuePair<string, string>>();
				postBody.Add(new KeyValuePair<string, string>("username", username));
				postBody.Add(new KeyValuePair<string, string>("password", password));

				var req = new HttpRequestMessage(
						HttpMethod.Post,
						"auth/logon"
					)
					{ Content = new FormUrlEncodedContent(postBody) };

				HttpResponseMessage response = await httpClient.SendAsync(req);
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateAuthenticationResponse(response, responseJson);
			}
		}

		public static ClaimsIdentity GetUserClaims()
		{
			return new ClaimsIdentity(
				new[]
				{
					// TODO: Append fields from JWT in responseContent to AuthResponse
					new Claim(ClaimTypes.NameIdentifier, "0099003"),
					new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

					new Claim(ClaimTypes.Name, "STEPHEN CASPERS".ToTitleCase()),
					new Claim(ClaimTypes.GivenName, "STEPHEN".ToTitleCase()),
					new Claim(ClaimTypes.Surname, "CASPERS".ToTitleCase()),
					new Claim(ClaimTypes.Actor, "scaspers" ),
					new Claim(ClaimTypes.UserData, ""),

					new Claim(ClaimTypes.Role, AuthorizationRoles.ADMIN),
					//new Claim(ClaimTypes.Role, authResponse.Roles.Any(r => r.EndsWith("Admin")) ? AuthorizationRoles.ADMIN : AuthorizationRoles.USER),

				}, DefaultAuthenticationTypes.ApplicationCookie); // TODO Use external bearer option???
		}


		private static AuthenticationResponse CreateAuthenticationResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			AuthenticationResponse authenticationResponse = new AuthenticationResponse();
			authenticationResponse.IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

				// THIS
				//var jwt = new JwtSecurityToken(loginResponse.authToken);

				// OR THIS

				var tokenHandler = new JwtSecurityTokenHandler();

				var validationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateAudience = false,
					ValidateIssuer = false,
					IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes("cThHM2VQOTNuUXo0MTdrTHhFMTk="))
				};

				var boolio = tokenHandler.CanReadToken(loginResponse.authToken);
				SecurityToken validatedToken;
				try
				{
					tokenHandler.ValidateToken(loginResponse.authToken, validationParameters, out validatedToken);
				}
				catch (Exception e)
				{
					Console.WriteLine("Fuck");
				}


				authenticationResponse.ClaimsIdentity = GetUserClaims();

				// TODO: Mutate future instances of HTTPClientService to append the username and password headers if always needed
				// Need this everywhere

				HttpClientService.AppendBearer = true;
				var bearerToken = loginResponse.refreshToken;
				HttpClientService.BearerToken = bearerToken;
			}
			else
			{
				authenticationResponse.ErrorMessage = responseContent;
				authenticationResponse.HTTPCode = httpResponse.StatusCode;
			}
			return authenticationResponse;
		}

	}
}
