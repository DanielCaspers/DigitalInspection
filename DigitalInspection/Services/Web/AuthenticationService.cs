﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models;
using DigitalInspection.Models.Web;
using DigitalInspection.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SecurityToken = Microsoft.IdentityModel.Tokens.SecurityToken;

namespace DigitalInspection.Services.Web
{
	public class AuthenticationService : HttpClientService
	{
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response
		public static async Task<HttpResponse<ClaimsIdentity>> Login(string username, string password)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				var postBody = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("username", username),
					new KeyValuePair<string, string>("password", password)
				};

				var req = new HttpRequestMessage(
						HttpMethod.Post,
						"auth/logon"
					)
					{ Content = new FormUrlEncodedContent(postBody) };

				HttpResponseMessage response = await httpClient.SendAsync(req);
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateLoginAuthenticationResponse(response, responseJson);
			}
		}

		public static async Task<HttpResponse<ClaimsIdentity>> Logout(IEnumerable<Claim> userClaims)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, false))
			{
				HttpResponseMessage response = await httpClient.GetAsync("auth/logoff");
				string responseJson = await response.Content.ReadAsStringAsync();

				return new HttpResponse<ClaimsIdentity>(response, responseJson);
			}
		}

		public static Task<ClaimsIdentity> RefreshIdentityAsync(ApplicationUserManager manager, ApplicationUser user)
		{
			// TODO GetClaim("MA-RefreshToken")
			return user.GenerateUserIdentityAsync(manager);
		}

		private static ClaimsIdentity GetUserClaims(JwtSecurityToken token, LoginResponse loginResponse)
		{
			var employeeId = token.Claims.Single(claim => claim.Type == "empID").Value;
			var authRole = GetAuthRole(token.Claims);
			var name = token.Claims.Single(claim => claim.Type == "firstName").Value.ToTitleCase();
			return new ClaimsIdentity(
				token.Claims.Concat(
					new[]
					{
						// First two needed for anti forgery token support with claims based auth
						new Claim(ClaimTypes.NameIdentifier, employeeId),
						new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

						new Claim(ClaimTypes.Role, authRole),
						new Claim(ClaimTypes.Name, name),
						new Claim("MA-AuthToken", loginResponse.authToken),
						new Claim("MA-RefreshToken", loginResponse.refreshToken)
					})
				, DefaultAuthenticationTypes.ApplicationCookie); // TODO Use external bearer option???
		}


		private static HttpResponse<ClaimsIdentity> CreateLoginAuthenticationResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			HttpResponse<ClaimsIdentity> authenticationResponse = new HttpResponse<ClaimsIdentity>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

				var tokenHandler = new JwtSecurityTokenHandler();

				var validationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateAudience = true,
					ValidateIssuer = true,
					ValidAudience = ConfigurationManager.AppSettings.Get("MurphyAutomotiveValidTokenAudience"),
					ValidIssuer = ConfigurationManager.AppSettings.Get("MurphyAutomotiveValidTokenIssuer"),
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.ASCII.GetBytes(
							ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppSecret")
						)
					)
				};

				SecurityToken validatedToken;
				try
				{
					tokenHandler.ValidateToken(loginResponse.authToken, validationParameters, out validatedToken);
				}
				catch (Exception)
				{
					// If this happens, fail the request and prevent login
					return authenticationResponse;
				}

				authenticationResponse.Entity = GetUserClaims(validatedToken as JwtSecurityToken, loginResponse);
			}

			return authenticationResponse;
		}

		private static string GetAuthRole(IEnumerable<Claim> claims)
		{
			var roleValues = claims.Where(claim => claim.Type == "roles").Select(r => r.Value).ToList();

			if (roleValues.Any(r => r.EndsWith(Roles.Admin)))
			{
				return Roles.Admin;
			}
			else if (roleValues.Any(r => r.EndsWith(Roles.LocationManager)))
			{
				return Roles.LocationManager;
			}
			else if (roleValues.Any(r => r.EndsWith(Roles.ServiceAdvisor)))
			{
				return Roles.ServiceAdvisor;
			}
			else if (roleValues.Any(r => r.EndsWith(Roles.Technician)))
			{
				return Roles.Technician;
			}
			else
			{
				return Roles.User;
			}
		}

	}
}
