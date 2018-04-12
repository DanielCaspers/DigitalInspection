using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;

namespace DigitalInspection.Services
{
	public class VehicleHistoryService: HttpClientService
	{
		// TODO: Find way to override static superclass, and instead only change ConstructBaseUri() method.
		//  Might require factory pattern for httpClient like DbContextFactories?
		protected static HttpClient InitializeHttpClient(IEnumerable<Claim> userClaims, string companyNumber)
		{
			var httpClient = HttpClientService.InitializeHttpClient(userClaims, false);

			string apiBaseUrl = ConfigurationManager.AppSettings.Get("MurphyAutomotiveD3apiBaseUrl").TrimEnd('/');

			httpClient.BaseAddress = new Uri($"{apiBaseUrl}/{companyNumber}/");

			return httpClient;
		}

		public static async Task<HttpResponse<IList<VehicleHistoryItem>>> GetVehicleHistory(IEnumerable<Claim> userClaims, string VIN, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				var url = $"vehiclehist/{VIN}";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateVehicleHistoryResponse(response, json);
			}
		}

		private static HttpResponse<IList<VehicleHistoryItem>> CreateVehicleHistoryResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			HttpResponse<IList<VehicleHistoryItem>> vehicleHistoryResponse = new HttpResponse<IList<VehicleHistoryItem>>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				try
				{
					var vehicleHistoryItems = JsonConvert.DeserializeObject<VehicleHistoryItemDTO[]>(responseContent);
					vehicleHistoryResponse.Entity = VehicleHistoryItemMapper.mapToVehicleHistoryItems(vehicleHistoryItems);
				}
				catch (Exception e)
				{
					vehicleHistoryResponse.ErrorMessage = e.Message;
				}
			}

			return vehicleHistoryResponse;
		}
	}
}
