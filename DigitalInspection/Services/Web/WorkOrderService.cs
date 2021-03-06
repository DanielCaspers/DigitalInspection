﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Mappers;
using DigitalInspection.Models.Orders;
using DigitalInspection.Models.Web;
using DigitalInspection.Utils;
using Newtonsoft.Json;

namespace DigitalInspection.Services.Web
{
	public class WorkOrderService : HttpClientService
	{
		// TODO: Find way to override static superclass, and instead only change ConstructBaseUri() method.
		//  Might require factory pattern for httpClient like DbContextFactories?
		protected static HttpClient InitializeHttpClient(IEnumerable<Claim> userClaims, string companyNumber)
		{
			var httpClient = HttpClientService.InitializeHttpClient(userClaims, false);

			var apiBaseUrl = ConfigurationManager.AppSettings.Get("MurphyAutomotiveD3apiBaseUrl").TrimEnd('/');

			httpClient.BaseAddress = new Uri($"{apiBaseUrl}/{companyNumber}/");

			return httpClient;
		}

		private const int DEFAULT_NUM_ORDERS = 100;
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response

		#region Get Multiple Orders
		public static async Task<IList<WorkOrder>> GetWorkOrders(IEnumerable<Claim> userClaims, string companyNumber, int numOrders = DEFAULT_NUM_ORDERS)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				var url = $"orders/?$top={numOrders}";
				var response = await httpClient.GetAsync(url);
				var json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForTech(IEnumerable<Claim> userClaims, string companyNumber)
		{
			var userClaimsList = userClaims.ToList();
			using (HttpClient httpClient = InitializeHttpClient(userClaimsList, companyNumber))
			{
				var url = $"orders/tech/{GetEmployeeNumber(userClaimsList)}/status/~blocked?$top={DEFAULT_NUM_ORDERS}";
				var response = await httpClient.GetAsync(url);
				var json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		public static async Task<IList<WorkOrder>> GetWorkOrdersForServiceAdvisor(IEnumerable<Claim> userClaims, string companyNumber)
		{
			var userClaimsList = userClaims.ToList();

			using (HttpClient httpClient = InitializeHttpClient(userClaimsList, companyNumber))
			{
				var url = $"orders/writer/{GetEmployeeNumber(userClaimsList)}?$top={DEFAULT_NUM_ORDERS}";
				var response = await httpClient.GetAsync(url);
				var json = await response.Content.ReadAsStringAsync();

				return GetWorkOrdersInternal(json);
			}
		}

		#endregion

		#region Get Single Work Order

		// For the overload without user claims, we must manually infer the company number from the order number
		public static async Task<HttpResponse<WorkOrder>> GetWorkOrder(string id)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				var url = $"{id.Substring(0,3)}/orders/{id}";
				var response = await httpClient.GetAsync(url);
				var json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		// For the overload with user claims, we grab the user's company number from their selected company via a cookie set for DI
		public static async Task<HttpResponse<WorkOrder>> GetWorkOrder(IEnumerable<Claim> userClaims, string id, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				var url = $"orders/{id}";
				var response = await httpClient.GetAsync(url);
				var json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		#endregion

		#region Save Work Order

		public static async Task<HttpResponse<WorkOrder>> SaveWorkOrder(IEnumerable<Claim> userClaims, WorkOrder order, string companyNumber)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				// Map work order to work order dto
				WorkOrderDTO dto = WorkOrderMapper.mapToWorkOrderDTO(order);

				// Serialize mapped object
				string requestJson = JsonConvert.SerializeObject(dto);
				var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				var url = $"orders/{order.Id}";
				var response = await httpClient.PatchAsync(url, httpContent);
				var responseJson = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, responseJson);
			}
		}

		public static async Task<HttpResponse<WorkOrder>> SaveWorkOrderNote(IEnumerable<Claim> userClaims, string workOrderId, string companyNumber, IList<string> notes)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				var url = $"orders/{workOrderId}";

				object noteDTO = new {orderNotes = notes, orderID = workOrderId};
				// Serialize mapped object
				string noteJson = JsonConvert.SerializeObject(noteDTO);
				var httpContent = new StringContent(noteJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await httpClient.PatchAsync(url, httpContent);
				string json = await response.Content.ReadAsStringAsync();

				return CreateWorkOrderResponse(response, json);
			}
		}

		#endregion

		#region Set Work Order Status

		public static async Task<HttpResponse<bool>> SetStatus(IEnumerable<Claim> userClaims, string id, string companyNumber, WorkOrderStatusCode status)
		{
			using (HttpClient httpClient = InitializeHttpClient(userClaims, companyNumber))
			{
				// TODO: DJC SAC SetStatus will eventually read content from the body. Agreed on 7/3/2019
				var url = $"orderstatus/{id}?newstatus={ ((int) status).ToString() }";

				// WARNING: This is only used to satisfy the HttpClient.PatchAsync() method signature. It was not working with newstatus in the FormUrlEncodedContent
				var formContent = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("", "")
				});

				var response = await httpClient.PatchAsync(url, formContent);
				var json = await response.Content.ReadAsStringAsync();

				HttpResponse<bool> httpResponse = new HttpResponse<bool>(response, json)
				{
					Entity = true
				};

				return httpResponse;
			}
		}

		#endregion

		#region Helpers

		private static IList<WorkOrder> GetWorkOrdersInternal(string json)
		{
			IList<WorkOrderDTO> orderDtos = JsonConvert.DeserializeObject<List<WorkOrderDTO>>(json);

			IList<WorkOrder> workOrders = new List<WorkOrder>();
			foreach (WorkOrderDTO orderDto in orderDtos)
			{
				workOrders.Add(WorkOrderMapper.mapToWorkOrder(orderDto));
			}
			return workOrders;
		}

		private static HttpResponse<WorkOrder> CreateWorkOrderResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			HttpResponse<WorkOrder> workOrderResponse = new HttpResponse<WorkOrder>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				WorkOrderDTO orderDto = JsonConvert.DeserializeObject<WorkOrderDTO>(responseContent);
				workOrderResponse.Entity = WorkOrderMapper.mapToWorkOrder(orderDto);
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