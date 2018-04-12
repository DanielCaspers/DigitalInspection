using DigitalInspection.Models.Store.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using DigitalInspection.Models.Store;

namespace DigitalInspection.Services
{
	public class StoreInfoService: HttpClientService
	{
		public static async Task<HttpResponse<StoreInfo>> GetStoreInfo(string companyNumber)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				string url = string.Format("conos/{0}", companyNumber);
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
