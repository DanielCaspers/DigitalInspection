using System;
using System.Collections.Generic;

namespace DigitalInspection.Models.Orders
{
	public class VehicleHistoryItem
	{
		public string OrderId { get; set; }

		/// <summary>
		/// An Inspection Id of the associated work order, if available.
		/// </summary>
		/// <remarks>
		/// Owned by DigitalInspection persistence, not D3-API
		/// </remarks>
		public Guid? InspectionId { get; set; }

		public DateTime? CompletionDate { get; set; }

		public int? VehicleOdometer { get; set; }

		public IList<string> LaborDescription { get; set; }

		public string InvoiceLink { get; set; }
	}
}
