using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DigitalInspection.Services
{
	public class HttpClientService
	{
		protected static HttpClient InitializeHttpClient(bool includeCompanyNumber = true)
		{
			var httpClient = new HttpClient();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			httpClient.BaseAddress = ConstructBaseUri(includeCompanyNumber);
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("x-appkey", ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppKey"));

			// TODO Remove when putting in real auth stack. This will be present on JWT returned from auth server. Only app key is necessary in web.config
			httpClient.DefaultRequestHeaders.Add("x-authtoken", ConfigurationManager.AppSettings.Get("MurphyAutomotiveAppSecret"));
			return httpClient;
		}

		private static Uri ConstructBaseUri(bool includeCompanyNumber)
		{
			string uri;
			string apiBaseUrl = ConfigurationManager.AppSettings.Get("MurphyAutomotiveD3apiBaseUrl").TrimEnd('/');

			if (includeCompanyNumber)
			{
				string companyNumber = ConfigurationManager.AppSettings.Get("MurphyAutomotiveCompanyNumber");
				uri = string.Format("{0}/{1}/", apiBaseUrl, companyNumber);
			}
			else
			{
				uri = string.Format("{0}/", apiBaseUrl);
			}				

			return new Uri(uri);
		}
	}
}