﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DigitalInspection.Services.Web
{
	public class HttpClientService
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

		protected static Uri ConstructBaseUri()
		{
			var apiBaseUrl = ConfigurationManager.AppSettings.Get("MurphyAutomotiveD3apiBaseUrl").TrimEnd('/');

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
	}
}