using System.Net.Http;
using System.Threading.Tasks;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Store;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class StoreInfoService: HttpClientService
	{
		public static async Task<HttpResponse<StoreInfo>> GetStoreInfo(string companyNumber)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				var url = $"conos/{companyNumber}";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateStoreInfoResponse(response, json);
			}
		}

		private static HttpResponse<StoreInfo> CreateStoreInfoResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			HttpResponse<StoreInfo> storeInfoResponse = new HttpResponse<StoreInfo>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				StoreInfoDTO dto = JsonConvert.DeserializeObject<StoreInfoDTO>(responseContent);
				storeInfoResponse.Entity = StoreInfoMapper.mapToStoreInfo(dto);
			}

			return storeInfoResponse;
		}
	}
}
