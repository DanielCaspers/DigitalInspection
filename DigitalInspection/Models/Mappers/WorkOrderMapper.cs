using DigitalInspection.Models.DTOs;
using DigitalInspection.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DigitalInspection.Models.Mappers
{
	public static class WorkOrderMapper
	{
		public static WorkOrder mapToWorkOrder(WorkOrderDTO dto)
		{
			WorkOrder order = new WorkOrder();

			order.Id = dto.orderId;
			order.Date = DateTimeUtils.FromUnixTime(dto.orderDate);
			//order.Status = (int) orderDto.orderStatus;

			order.Customer = new Customer();
			order.Customer.Id = dto.clientId;
			order.Customer.Name = dto.clientName.ToTitleCase();

			order.Customer.Address = new Address();
			order.Customer.Address.Line1 = dto.clientAddr.ToTitleCase();
			order.Customer.Address.Line2 = dto.clientAddr2.ToTitleCase();
			order.Customer.Address.City = dto.clientCity.ToTitleCase();
			order.Customer.Address.State = dto.clientState;
			order.Customer.Address.ZIP = dto.clientZip;

			order.Customer.PhoneNumbers = new List<PhoneNumber>();
			foreach (ClientPhoneDTO clientPhoneDto in dto.clientPhone)
			{
				order.Customer.PhoneNumbers.Add(mapToPhoneNumber(clientPhoneDto));
			}

			order.Vehicle = new Vehicle();
			order.Vehicle.VIN = dto.vehicleId;
			order.Vehicle.Year = dto.vehicleYear;
			order.Vehicle.Make = dto.vehicleMake.ToTitleCase();
			order.Vehicle.Model = dto.vehicleModel.ToTitleCase();
			order.Vehicle.License = dto.vehicleLicense;
			order.Vehicle.Color = dto.vehicleColor.ToTitleCase();

			if (Color.FromName(order.Vehicle.Color).IsKnownColor == false)
			{
				order.Vehicle.Color = null;
			}
			order.Vehicle.Engine = dto.vehicleEngine;
			order.Vehicle.Transmission = dto.vehicleTransmission.ToTitleCase();
			order.Vehicle.Odometer = dto.vehicleOdometer;
			return order;
		}

		public static PhoneNumber mapToPhoneNumber(ClientPhoneDTO dto)
		{
			PhoneNumber phoneNumber = new PhoneNumber();
			phoneNumber.Number = dto.number;
			phoneNumber.ContactName = dto.name;
			phoneNumber.Type = dto.type;
			return phoneNumber;
		}

		public static WorkOrderDTO mapToWorkOrderDTO(WorkOrder order)
		{
			WorkOrderDTO dto = new WorkOrderDTO();

			dto.orderId = order.Id;
			dto.orderDate = DateTimeUtils.ToUnixTime(order.Date);

			dto.clientId = order.Customer.Id;
			dto.clientName = order.Customer.Name.ToUpper();

			dto.clientAddr = order.Customer.Address.Line1.ToUpper();
			dto.clientAddr2 = order.Customer.Address.Line2.ToUpper();
			dto.clientCity = order.Customer.Address.City.ToUpper();
			dto.clientState = order.Customer.Address.State;
			dto.clientZip = order.Customer.Address.ZIP;

			dto.clientPhone = new ClientPhoneDTO[order.Customer.PhoneNumbers.Count];
			for(int i = 0; i < order.Customer.PhoneNumbers.Count; i++)
			{
				dto.clientPhone[i] = mapToPhoneNumberDTO(order.Customer.PhoneNumbers[i]);
			}

			dto.vehicleId = order.Vehicle.VIN;
			dto.vehicleYear = order.Vehicle.Year;
			dto.vehicleMake = order.Vehicle.Make.ToUpper();
			dto.vehicleModel = order.Vehicle.Model.ToUpper();
			dto.vehicleLicense = order.Vehicle.License;
			dto.vehicleColor = order.Vehicle.Color.ToUpper();
			dto.vehicleEngine = order.Vehicle.Engine;
			dto.vehicleTransmission = order.Vehicle.Transmission.ToUpper();
			dto.vehicleOdometer = order.Vehicle.Odometer;

			return dto;
		}

		public static ClientPhoneDTO mapToPhoneNumberDTO(PhoneNumber phoneNumber)
		{
			ClientPhoneDTO dto = new ClientPhoneDTO();
			dto.number = phoneNumber.Number;
			dto.name = phoneNumber.ContactName;
			dto.type = phoneNumber.Type;
			return dto;
		}
	}
}