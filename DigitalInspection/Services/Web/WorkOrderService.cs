using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigitalInspection.Services
{
	public class WorkOrderService : HttpClientService
	{
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response
		public static async Task<IList<WorkOrder>> GetWorkOrders(int numOrders = 20)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				string url = string.Format("orders/?$top={0}", numOrders);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForTech(string employeeNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				string url = string.Format("orders/tech/{0}", employeeNumber);
				var response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<WorkOrderResponse> GetWorkOrder(string id, bool requestlock = false)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				string url = string.Format("orders/{0}?$requestlock={1}", id, Convert.ToInt32(requestlock));
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		public static async Task<WorkOrderResponse> SaveWorkOrder(WorkOrder order, bool releaselockonly = false)
		{
			using (HttpClient httpClient = InitializeHttpClient())
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

		private static WorkOrderResponse CreateWorkOrderResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			WorkOrderResponse workOrderResponse = new WorkOrderResponse();
			workOrderResponse.IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				WorkOrderDTO orderDto = JsonConvert.DeserializeObject<WorkOrderDTO>(responseContent);
				workOrderResponse.WorkOrder = WorkOrderMapper.mapToWorkOrder(orderDto);
			}
			else
			{
				workOrderResponse.ErrorMessage = responseContent;
				workOrderResponse.HTTPCode = httpResponse.StatusCode;
			}
			return workOrderResponse;
		}

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

	}
}