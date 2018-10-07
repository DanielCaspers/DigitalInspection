using System;
using System.Net.Http;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;

namespace DigitalInspection.Services.Web
{
	public class InspectionHttpService: HttpClientService<Inspection>
	{
		// TODO This type is woefully wrong....
		public static async Task<HttpResponse<string>> GetInspectionReport(string workOrderId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"WorkOrders/{workOrderId}/Inspection";

				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				var entityResponse = new HttpResponse<string>(response, json)
				{
					Entity = json
				};

				return entityResponse;
			}
		}

		// TODO This type is woefully wrong....
		public static async Task<HttpResponse<string>> GetInspectionReport(Guid inspectionId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Inspections/{inspectionId}";

				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				var entityResponse = new HttpResponse<string>(response, json)
				{
					Entity = json
				};

				return entityResponse;
			}
		}

		public static async Task<HttpResponse<Guid>> GetInspectionId(string workOrderId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"WorkOrders/{workOrderId}/InspectionId";

				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse<Guid>(response, json);
			}
		}

		public static async Task<HttpResponse<string>> GetWorkOrderId(Guid inspectionId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Inspections/{inspectionId}/WorkOrderId";

				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse<string>(response, json);
			}
		}

		public static async Task<HttpResponse<Inspection>> Delete(string workOrderId)
		{
			return await DeleteEntity($"WorkOrders/{workOrderId}/Inspection");
		}
	}
}
