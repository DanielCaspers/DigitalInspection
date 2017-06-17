using AutoMapper;
using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
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
				var response = await httpClient.GetAsync(string.Format("orders/?$top=${0}", numOrders));
				string json = await response.Content.ReadAsStringAsync();

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

		public static async Task<WorkOrder> GetWorkOrder(string id)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				var response = await httpClient.GetAsync(string.Format("orders/{0}", id));
				string json = await response.Content.ReadAsStringAsync();

				WorkOrderDTO orderDto = JsonConvert.DeserializeObject<WorkOrderDTO>(json);
				return WorkOrderMapper.mapToWorkOrder(orderDto);
			}
		}

		public static async Task<WorkOrder> SaveWorkOrder(WorkOrder order)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				// Map work order to work order dto
				WorkOrderDTO dto = WorkOrderMapper.mapToWorkOrderDTO(order);
				// Serialize mapped object
				string json = JsonConvert.SerializeObject(dto);
				var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await httpClient.PutAsync(string.Format("orders/{0}", order.Id), httpContent);
				//string json = await response.Content.ReadAsStringAsync();

				WorkOrderDTO orderDto = JsonConvert.DeserializeObject<WorkOrderDTO>(json);
				return WorkOrderMapper.mapToWorkOrder(orderDto);
			}
		}
	}
}