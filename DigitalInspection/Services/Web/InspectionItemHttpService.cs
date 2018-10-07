using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Orders;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class InspectionItemHttpService: HttpClientService<InspectionItem>
	{
		public static async Task<HttpResponse<InspectionItem>> GetById(Guid id)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{id}";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionItem>> SetCannedResponses(Guid inspectionItemId, IEnumerable<Guid> selectedCannedResponseIds)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/CannedResponses";
				string requestJson = JsonConvert.SerializeObject(selectedCannedResponseIds);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionItem>> SetCondition(Guid inspectionItemId, RecommendedServiceSeverity condition)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/Condition";
				string requestJson = JsonConvert.SerializeObject(condition);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionItem>> SetCustomerConcern(Guid inspectionItemId, bool isCustomerConcern)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/CustomerConcern";
				string requestJson = JsonConvert.SerializeObject(isCustomerConcern);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionItem>> SetNotes(Guid inspectionItemId, string notes)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/Notes";
				string requestJson = JsonConvert.SerializeObject(notes);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<InspectionItem>> SetMeasurements(Guid inspectionItemId, IEnumerable<UpdateInspectionMeasurementRequest> measurementUpdates)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"InspectionItems/{inspectionItemId}/Measurements";
				string requestJson = JsonConvert.SerializeObject(measurementUpdates);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}
	}
}
