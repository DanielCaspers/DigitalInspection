using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DigitalInspection.Services
{
	public class WorkOrderService : HttpClientService
	{
		private const int DEFAULT_NUM_ORDERS = 60;
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response

		#region Get Multiple Orders
		public static async Task<IList<WorkOrder>> GetWorkOrders(IEnumerable<Claim> userClaims, int numOrders = DEFAULT_NUM_ORDERS)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				string url = string.Format("orders/?$top={0}", numOrders);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForTech(IEnumerable<Claim> userClaims)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				string url = string.Format("orders/tech/{0}/status/~blocked?$top={1}", GetEmployeeNumber(userClaims), DEFAULT_NUM_ORDERS);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForServiceAdvisor(IEnumerable<Claim> userClaims)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				string url = string.Format("orders/writer/{0}?$top={1}", GetEmployeeNumber(userClaims), DEFAULT_NUM_ORDERS);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		#endregion

		#region Get Single Work Order

		// For the overload without user claims, we must manually infer the company number from the order number
		public static async Task<WorkOrderResponse> GetWorkOrder(string id, bool requestlock = false)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				string url = string.Format("{0}/orders/{1}?$requestlock={2}", id.Substring(0,3), id, Convert.ToInt32(requestlock));
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		// For the overload with user claims, we grab the user's company number from their employee ID
		public static async Task<WorkOrderResponse> GetWorkOrder(IEnumerable<Claim> userClaims, string id, bool requestlock = false)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				string url = string.Format("orders/{0}?$requestlock={1}", id, Convert.ToInt32(requestlock));
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		#endregion

		#region Save Work Order

		public static async Task<WorkOrderResponse> ReleaseLock(IEnumerable<Claim> userClaims, string orderId)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				var httpContent = new StringContent("", Encoding.UTF8, "application/json");

				string url = string.Format("orders/{0}?$releaselockonly={1}", orderId, 1);
				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, responseJson);
			}
		}

		public static async Task<WorkOrderResponse> SaveWorkOrder(IEnumerable<Claim> userClaims, WorkOrder order, bool releaselockonly = false)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims))
			{
				// Map work order to work order dto
				WorkOrderDTO dto = WorkOrderMapper.mapToWorkOrderDTO(order);

				// Serialize mapped object
				string requestJson = JsonConvert.SerializeObject(dto);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				string url = string.Format("orders/{0}?$releaselockonly={1}", order.Id, Convert.ToInt32(releaselockonly));
				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, responseJson);
			}
		}

		#endregion

		#region Helpers

		private static IList<WorkOrder> GetWorkOrdersInternal(string json)
		{
			IList<WorkOrderDTO> orderDtos = new List<WorkOrderDTO>();
			orderDtos = JsonConvert.DeserializeObject<List<WorkOrderDTO>>(json);

			IList<WorkOrder> workOrders = new List<WorkOrder>();
			foreach (WorkOrderDTO orderDto in orderDtos)
			{
				workOrders.Add(WorkOrderMapper.mapToWorkOrder(orderDto));
			}
			return workOrders;
		}

		private static WorkOrderResponse CreateWorkOrderResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			WorkOrderResponse workOrderResponse = new WorkOrderResponse
			{
				IsSuccessStatusCode = httpResponse.IsSuccessStatusCode,
				HTTPCode = httpResponse.StatusCode,
				ErrorMessage = httpResponse.IsSuccessStatusCode ? "" : responseContent
			};

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				WorkOrderDTO orderDto = JsonConvert.DeserializeObject<WorkOrderDTO>(responseContent);
				workOrderResponse.WorkOrder = WorkOrderMapper.mapToWorkOrder(orderDto);
			}

			return workOrderResponse;
		}

		private static string GetEmployeeNumber(IEnumerable<Claim> userClaims)
		{
			// Takes 4 digit employee number. 7 digit will fail
			return userClaims
				.Where(c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value.Substring(3))
				.Single();
		}

		#endregion
	}
}