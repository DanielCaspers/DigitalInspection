using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels.Tags;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class TagHttpService: HttpClientService<Tag>
	{
		public static async Task<HttpResponse<IEnumerable<Tag>>> GetTags()
		{
			return await GetEntities($"Tags/");
		}

		public static async Task<HttpResponse<IEnumerable<Tag>>> GetEmployeeVisibleTags()
		{
			return await GetEntities($"Tags/VisibleToEmployee");
		}

		public static async Task<HttpResponse<Tag>> GetTag(Guid id)
		{
			return await GetEntity($"Tags/{id}");
		}

		public static async Task<HttpResponse<Tag>> UpdateTag(Guid id, AddTagViewModel tag)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Tags/{id}";
				string requestJson = JsonConvert.SerializeObject(tag);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<Tag>> CreateTag(AddTagViewModel tag)
		{
			using (HttpClient httpClient = InitializeDINetCoreHttpClient())
			{
				var url = $"Tags/";
				string requestJson = JsonConvert.SerializeObject(tag);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateDomainEntityResponse(response, json);
			}
		}

		public static async Task<HttpResponse<Tag>> DeleteTag(Guid id)
		{
			return await DeleteEntity($"Tags/{id}");
		}
	}
}
