using DigitalInspection.Models.DTOs;
using DigitalInspection.Models.Orders;
using DigitalInspection.Utils;
using System.Collections.Generic;
using System.Linq;

namespace DigitalInspection.Models.Mappers
{
	public static class WorkOrderMapper
	{
		public static WorkOrder mapToWorkOrder(WorkOrderDTO dto)
		{
			WorkOrder order = new WorkOrder();

			order.Id = dto.orderID;
			order.Date = DateTimeUtils.FromUnixTime(dto.orderDate);
			order.ScheduleDate = DateTimeUtils.FromUnixTime(dto.schedDate);
			order.CompletionDate = DateTimeUtils.FromUnixTime(dto.completionDate);
			order.EmployeeId = dto.techNum;
			order.WorkDescription = dto.workDesc;

			order.Status = new WorkOrderStatus(
				dto.orderStatus.statusCode,
				dto.orderStatus.statusDesc,
				dto.orderStatus.statusTimestamp,
				dto.orderStatus.statusMisc);

			Address clientAddress = new Address(
				dto.clientAddr,
				dto.clientAddr2,
				dto.clientCity,
				dto.clientState,
				dto.clientZip);

			IList<PhoneNumber> clientPhoneNumbers;
			if (dto.clientPhone == null)
			{
				clientPhoneNumbers = null;
			}
			else
			{
				clientPhoneNumbers = new List<PhoneNumber>();
				foreach (ClientPhoneDTO clientPhoneDto in dto.clientPhone)
				{
					clientPhoneNumbers.Add(new PhoneNumber(
						clientPhoneDto.number,
						clientPhoneDto.name,
						clientPhoneDto.type,
						clientPhoneDto.smsPrefs)
					);
				}
			}

			order.Customer = new Customer(
				dto.clientID,
				dto.clientName,
				clientAddress,
				clientPhoneNumbers);

			order.Vehicle = new Vehicle(
				dto.vehicleID,
				dto.vehicleYear,
				dto.vehicleMake,
				dto.vehicleModel,
				dto.vehicleLicense,
				dto.vehicleColor,
				dto.vehicleEngine,
				dto.vehicleTransmission,
				dto.vehicleOdometer);

			return order;
		}

		public static WorkOrderDTO mapToWorkOrderDTO(WorkOrder order)
		{
			WorkOrderDTO dto = new WorkOrderDTO();

			dto.orderID = order.Id;
			dto.orderDate = DateTimeUtils.ToUnixTime(order.Date);
			dto.schedDate = DateTimeUtils.ToUnixTime(order.ScheduleDate);
			dto.completionDate = DateTimeUtils.ToUnixTime(order.CompletionDate);
			dto.techNum = order.EmployeeId;
			dto.workDesc = order.WorkDescription.ToArray();

			dto.orderStatus = new WorkOrderStatusDTO(
				order.Status.Code,
				order.Status.Description,
				order.Status.Timestamp,
				order.Status.Misc);

			dto.clientID = order.Customer.Id;
			dto.clientName = order.Customer.Name.ToUpper();

			dto.clientAddr = order.Customer.Address.Line1?.ToUpper();
			dto.clientAddr2 = order.Customer.Address.Line2?.ToUpper();
			dto.clientCity = order.Customer.Address.City.ToUpper();
			dto.clientState = order.Customer.Address.State;
			dto.clientZip = order.Customer.Address.ZIP;

			if (order.Customer.PhoneNumbers == null)
			{
				dto.clientPhone = null;
			}
			else
			{
				dto.clientPhone = new ClientPhoneDTO[order.Customer.PhoneNumbers.Count];
				for (int i = 0; i < order.Customer.PhoneNumbers.Count; i++)
				{
					dto.clientPhone[i] = new ClientPhoneDTO(
						order.Customer.PhoneNumbers[i].Number,
						order.Customer.PhoneNumbers[i].ContactName,
						order.Customer.PhoneNumbers[i].Type,
						order.Customer.PhoneNumbers[i].SMSPreferences
					);
				}
			}

			dto.vehicleID = order.Vehicle.VIN;
			dto.vehicleYear = order.Vehicle.Year;
			dto.vehicleMake = order.Vehicle.Make?.ToUpper();
			dto.vehicleModel = order.Vehicle.Model?.ToUpper();
			dto.vehicleLicense = order.Vehicle.License?.ToUpper();
			dto.vehicleColor = order.Vehicle.Color?.ToUpper();
			dto.vehicleEngine = order.Vehicle.Engine;
			dto.vehicleTransmission = order.Vehicle.Transmission?.ToUpper();
			dto.vehicleOdometer = order.Vehicle.Odometer;

			return dto;
		}
	}
}