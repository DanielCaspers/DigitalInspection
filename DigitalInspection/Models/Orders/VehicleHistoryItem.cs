using System;
using System.Collections.Generic;

namespace DigitalInspection.Models.Orders
{
	public class VehicleHistoryItem
	{
		public string OrderId { get; set; }

		public DateTime? CompletionDate { get; set; }

		public int? VehicleOdometer { get; set; }

		public IList<string> LaborDescription { get; set; }

		public string InvoiceLink { get; set; }
	}
}
