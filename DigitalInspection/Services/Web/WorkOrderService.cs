using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DigitalInspection.Services
{
	public class WorkOrderService : HttpClientService
	{
		// TODO: Find way to override static superclass, and instead only change ConstructBaseUri() method.
		//  Might require factory pattern for httpClient like DbContextFactories?
		protected static HttpClient InitializeHttpClient(IEnumerable<Claim> userClaims, string companyNumber)
		{
			var httpClient = HttpClientService.InitializeHttpClient(userClaims, false);

			string apiBaseUrl = ConfigurationManager.AppSettings.Get("MurphyAutomotiveD3apiBaseUrl").TrimEnd('/');

			httpClient.BaseAddress = new Uri($"{apiBaseUrl}/{companyNumber}/");

			return httpClient;
		}

		private const int DEFAULT_NUM_ORDERS = 60;
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response

		#region Get Multiple Orders
		public static async Task<IList<WorkOrder>> GetWorkOrders(IEnumerable<Claim> userClaims, string companyNumber, int numOrders = DEFAULT_NUM_ORDERS)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				string url = string.Format("orders/?$top={0}", numOrders);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForTech(IEnumerable<Claim> userClaims, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				string url = string.Format("orders/tech/{0}/status/~blocked?$top={1}", GetEmployeeNumber(userClaims), DEFAULT_NUM_ORDERS);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForServiceAdvisor(IEnumerable<Claim> userClaims, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
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

		// For the overload with user claims, we grab the user's company number from their selected company via a cookie set for DI
		public static async Task<WorkOrderResponse> GetWorkOrder(IEnumerable<Claim> userClaims, string id, string companyNumber, bool requestlock = false)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				string url = string.Format("orders/{0}?$requestlock={1}", id, Convert.ToInt32(requestlock));
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		#endregion

		#region Save Work Order

		public static async Task<WorkOrderResponse> ReleaseLock(IEnumerable<Claim> userClaims, string orderId, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				var httpContent = new StringContent("", Encoding.UTF8, "application/json");

				string url = string.Format("orders/{0}?$releaselockonly={1}", orderId, 1);
				HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, responseJson);
			}
		}

		public static async Task<WorkOrderResponse> SaveWorkOrder(IEnumerable<Claim> userClaims, WorkOrder order, string companyNumber, bool releaselockonly = false)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
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

		public static async Task<WorkOrderResponse> SaveWorkOrderNote(IEnumerable<Claim> userClaims, string workOrderId, string companyNumber, IList<string> notes)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				string url = string.Format("orders/{0}?$requestlock={1}", workOrderId, 1);
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				WorkOrderResponse workOrderResponse = CreateWorkOrderResponse(response, json);

				if (workOrderResponse.IsSuccessStatusCode == false)
				{
					return workOrderResponse;
				}

				object noteDTO = new {orderNotes = notes, orderID = workOrderId};
				// Serialize mapped object
				string noteJson = JsonConvert.SerializeObject(noteDTO);
				var httpContent = new StringContent(noteJson, Encoding.UTF8, "application/json");

				url = string.Format("orders/{0}?$releaselockonly={1}", workOrderId, 0);
				response = await httpClient.PutAsync(url, httpContent);
				json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
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