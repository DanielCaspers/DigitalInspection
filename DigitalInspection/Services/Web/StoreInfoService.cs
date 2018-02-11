using DigitalInspection.Models.Store.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DigitalInspection.Services
{
	public class StoreInfoService: HttpClientService
	{
		public static async Task<StoreInfoResponse> GetStoreInfo(string companyNumber)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				string url = string.Format("conos/{0}", companyNumber);
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateStoreInfoResponse(response, json);
			}
		}

		private static StoreInfoResponse CreateStoreInfoResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			StoreInfoResponse storeInfoResponse = new StoreInfoResponse
			{
				IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
				HTTPCode = httpResponse.StatusCode,
				ErrorMessage = httpResponse.IsSuccessStatusCode ? "" : responseContent
			};

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				StoreInfoDTO dto = JsonConvert.DeserializeObject<StoreInfoDTO>(responseContent);
				storeInfoResponse.StoreInfo = StoreInfoMapper.mapToStoreInfo(dto);
			}

			return storeInfoResponse;
		}
	}
}
