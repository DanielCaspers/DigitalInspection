using AutoMapper;
using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
					workOrders.Add(mapWorkOrder(orderDto));
				}
				return workOrders;
			}
		}

		private static WorkOrder mapWorkOrder(WorkOrderDTO orderDto)
		{
			WorkOrder actualOrder = new WorkOrder();

			actualOrder.Id = orderDto.orderId.Substring(3);
			actualOrder.Date = Convert.ToDateTime(DateTimeUtils.FromUnixTime(orderDto.orderDate));
			//actualOrder.Status = (int) orderDto.orderStatus;

			actualOrder.Customer = new Customer();
			actualOrder.Customer.Id = orderDto.clientId;
			actualOrder.Customer.Name = orderDto.clientName.ToTitleCase();

			actualOrder.Customer.Address = new Address();
			actualOrder.Customer.Address.Line1 = orderDto.clientAddr;
			actualOrder.Customer.Address.Line2 = orderDto.clientAddr2;
			actualOrder.Customer.Address.City = orderDto.clientCity;
			actualOrder.Customer.Address.State = orderDto.clientState;
			actualOrder.Customer.Address.ZIP = orderDto.clientZip;

			actualOrder.Customer.PhoneNumbers = new List<PhoneNumber>();
			foreach (ClientPhoneDTO clientPhoneDto in orderDto.clientPhone)
			{
				actualOrder.Customer.PhoneNumbers.Add(mapPhoneNumber(clientPhoneDto));
			}

			actualOrder.Vehicle = new Vehicle();
			actualOrder.Vehicle.VIN = orderDto.vehicleId;

			actualOrder.Vehicle.Year = orderDto.vehicleYear;

			actualOrder.Vehicle.Make = orderDto.vehicleMake.ToTitleCase();
			actualOrder.Vehicle.Model = orderDto.vehicleModel.ToTitleCase();
			actualOrder.Vehicle.License = orderDto.vehicleLicense;
			actualOrder.Vehicle.Color = orderDto.vehicleColor;
			actualOrder.Vehicle.Engine = orderDto.vehicleEngine;
			//actualOrder.Vehicle.Transmission = orderDto.vehicleTransmission;
			actualOrder.Vehicle.Odometer = orderDto.vehicleOdometer;
			return actualOrder;
		}

		private static PhoneNumber mapPhoneNumber(ClientPhoneDTO phoneDto)
		{
			PhoneNumber actualNumber = new PhoneNumber();
			actualNumber.Number = phoneDto.number;
			actualNumber.ContactName = phoneDto.name;
			actualNumber.Type = phoneDto.type;
			return actualNumber;
		}
	}
}