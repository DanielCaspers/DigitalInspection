using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class HttpClientService<T>
	{
		protected static HttpClient InitializeAnonymousHttpClient()
		{
			var httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			httpClient.BaseAddress = ConstructBaseUri();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("x-appkey", ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppKeyAnonymousAccess"));

			return httpClient;
		}

		protected static HttpClient InitializeHttpClient()
		{
			var httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			httpClient.BaseAddress = ConstructBaseUri();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("x-appkey", ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppKey"));

			var bearerToken = GetClaim("MA-AuthToken")?.Value;
			if (bearerToken != null)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
			}

			return httpClient;
		}

		protected static HttpClient InitializeHttpClient(IEnumerable<Claim> userClaims, bool includeCompanyNumber = true)
		{
			var httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			httpClient.BaseAddress = ConstructBaseUri();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("x-appkey", ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppKey"));

			var bearerToken = GetClaim(userClaims, "MA-AuthToken")?.Value;
			if (bearerToken != null)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
			}

			return httpClient;
		}

		protected static HttpClient InitializeDINetCoreHttpClient()
		{
			var httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			httpClient.BaseAddress = ConstructBaseUri("DigitalInspection-NetCoreBaseUrl");
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return httpClient;
		}

		protected static Uri ConstructBaseUri(string appSettingsKey = "MurphyAutomotiveD3apiBaseUrl")
		{
			var apiBaseUrl = ConfigurationManager.AppSettings.Get(appSettingsKey).TrimEnd('/');

			var uri = $"{apiBaseUrl}/";

			return new Uri(uri);
		}

		protected static Claim GetClaim(string claimName)
		{
			var claimsPrincipal = ClaimsPrincipal.Current;
			var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
			var claim = claimsIdentity.FindFirst(claimName);
			if (claim == null && claimName != "MA-AuthToken")
			{
				throw new Exception("Thread unable to access user's claims for claim: " + claimName);
			}

			return claimsIdentity.FindFirst(claimName);
		}

		protected static Claim GetClaim(IEnumerable<Claim> userClaims, string claimName)
		{
			var claim = userClaims.Single(c => c.Type == claimName);
			if (claim == null && claimName != "MA-AuthToken")
			{
				throw new Exception("Unable to access claims for user despite passing them into the async void" + claimName);
			}

			return claim;
		}

		#region DI Net Core Client

		protected static async Task<HttpResponse<IEnumerable<T>>> GetEntities(string url)
		{
			using (var httpClient = InitializeDINetCoreHttpClient())
			{
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityListResponse(response, json);
			}
		}

		protected static async Task<HttpResponse<T>> GetEntity(string url)
		{
			using (var httpClient = InitializeDINetCoreHttpClient())
			{
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<T>> DeleteEntity(string url)
		{
			using (var httpClient = InitializeDINetCoreHttpClient())
			{
				HttpResponseMessage response = await httpClient.DeleteAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		protected static HttpResponse<T> CreateDomainEntityResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			var entityResponse = new HttpResponse<T>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var entity = JsonConvert.DeserializeObject<T>(responseContent);
				entityResponse.Entity = entity;
			}

			return entityResponse;
		}

		protected static HttpResponse<IEnumerable<T>> CreateDomainEntityListResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			var entitiesResponse = new HttpResponse<IEnumerable<T>>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var entities = JsonConvert.DeserializeObject<IEnumerable<T>>(responseContent);
				entitiesResponse.Entity = entities;
			}

			return entitiesResponse;
		}

		#endregion
	}
}