using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels.ChecklistItems;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class ChecklistItemHttpService: HttpClientService<ChecklistItem>
	{
		public static async Task<HttpResponse<IEnumerable<ChecklistItem>>> GetChecklistItems()
		{
			return await GetEntities($"ChecklistItems/");
		}

		public static async Task<HttpResponse<ChecklistItem>> GetById(Guid id)
		{
			return await GetEntity($"ChecklistItems/{id}");
		}

		public static async Task<HttpResponse<EditChecklistItemViewModel>> GetEdit(Guid id)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"ChecklistItems/{id}/Edit";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateEditResponse(response, json);
			}
		}

		public static async Task<HttpResponse<ChecklistItem>> UpdateChecklistItem(Guid id, EditChecklistItemViewModel checklist)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"ChecklistItems/{id}";
				string requestJson = JsonConvert.SerializeObject(checklist);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<ChecklistItem>> CreateChecklistItem(AddChecklistItemViewModel checklist)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"ChecklistItems/";
				string requestJson = JsonConvert.SerializeObject(checklist);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<ChecklistItem>> DeleteChecklistItem(Guid id)
		{
			return await DeleteEntity($"ChecklistItems/{id}");
		}

		public static async Task<HttpResponse<CannedResponse>> CreateCannedResponse(Guid checklistItemId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"ChecklistItems/{checklistItemId}/CannedResponses";
				string requestJson = JsonConvert.SerializeObject(checklistItemId);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				var entityResponse = new HttpResponse<CannedResponse>(response, json);

				if (entityResponse.IsSuccessStatusCode && json != string.Empty)
				{
					var entity = JsonConvert.DeserializeObject<CannedResponse>(json);
					entityResponse.Entity = entity;
				}

				return entityResponse;
			}
		}

		public static async Task<HttpResponse<ChecklistItem>> DeleteCannedResponse(Guid cannedResponseId)
		{
			return await DeleteEntity($"ChecklistItems/CannedResponses/{cannedResponseId}");
		}

		public static async Task<HttpResponse<Measurement>> CreateMeasurement(Guid checklistItemId)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"ChecklistItems/{checklistItemId}/Measurements";
				string requestJson = JsonConvert.SerializeObject(checklistItemId);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				var entityResponse = new HttpResponse<Measurement>(response, json);

				if (entityResponse.IsSuccessStatusCode && json != string.Empty)
				{
					var entity = JsonConvert.DeserializeObject<Measurement>(json);
					entityResponse.Entity = entity;
				}

				return entityResponse;
			}
		}

		public static async Task<HttpResponse<ChecklistItem>> DeleteMeasurement(Guid measurementId)
		{
			return await DeleteEntity($"ChecklistItems/Measurements/{measurementId}");
		}

		private static HttpResponse<EditChecklistItemViewModel> CreateEditResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			var editResponse = new HttpResponse<EditChecklistItemViewModel>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var checklist = JsonConvert.DeserializeObject<EditChecklistItemViewModel>(responseContent);
				editResponse.Entity = checklist;
			}

			return editResponse;
		}
	}
}
