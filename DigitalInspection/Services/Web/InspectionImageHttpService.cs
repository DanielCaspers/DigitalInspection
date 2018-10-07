using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels.Inspections;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class InspectionImageHttpService: HttpClientService<InspectionImage>
	{
		public static async Task<HttpResponse<InspectionImage>> Upload(Guid inspectionItemId, HttpPostedFileBase image)
		{
			throw new NotImplementedException("Work in progress");
			//using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			//{
			//	var url = $"InspectionItems/{inspectionItemId}/InspectionImages/";
			//	using (var content = new MultipartFormDataContent())
			//	{
			//		content.Add();
			//		var fileContent = image.InputStream.
			//	}
			//	var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

			//	HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
			//	string json = await response.Content.ReadAsStringAsync();

			//	return CreateDomainEntityResponse(response, json);
			//}
		}

		public static async Task<HttpResponse<InspectionImage>> SetVisibility(Guid inspectionItemId, Guid inspectionImageId, bool isVisibleToCustomer)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/InspectionImages/{inspectionImageId}/Visibility";
				string requestJson = JsonConvert.SerializeObject(isVisibleToCustomer);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionImage>> Delete(Guid inspectionItemId, Guid inspectionImageId)
		{
			return await DeleteEntity($"InspectionItems/{inspectionItemId}/InspectionImages/{inspectionImageId}");
		}
	}
}
