using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels.Checklists;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class ChecklistHttpService: HttpClientService<Checklist>
	{
		public static async Task<HttpResponse<IEnumerable<Checklist>>> GetChecklists()
		{
			return await GetEntities($"Checklists/");
		}

		public static async Task<HttpResponse<Checklist>> GetById(Guid id)
		{
			return await GetEntity($"Checklists/{id}");
		}

		public static async Task<HttpResponse<EditChecklistViewModel>> GetEdit(Guid id)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Checklists/{id}/Edit";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateEditResponse(response, json);
			}
		}

		public static async Task<HttpResponse<Checklist>> Update(Guid id, EditChecklistViewModel checklist)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Checklists/{id}";
				string requestJson = JsonConvert.SerializeObject(checklist);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<Checklist>> Create(AddChecklistViewModel checklist)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Checklists/";
				string requestJson = JsonConvert.SerializeObject(checklist);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<Checklist>> Delete(Guid id)
		{
			return await DeleteEntity($"Checklists/{id}");
		}

		private static HttpResponse<EditChecklistViewModel> CreateEditResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			var editResponse = new HttpResponse<EditChecklistViewModel>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				var checklist = JsonConvert.DeserializeObject<EditChecklistViewModel>(responseContent);
				editResponse.Entity = checklist;
			}

			return editResponse;
		}
	}
}
