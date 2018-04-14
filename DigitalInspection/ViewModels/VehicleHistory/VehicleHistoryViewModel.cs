using System.Collections.Generic;
using DigitalInspection.Models.Orders;

namespace DigitalInspection.ViewModels.VehicleHistory
{
	public class VehicleHistoryViewModel
	{
		public IList<VehicleHistoryItem> VehicleHistory { get; set; } = new List<VehicleHistoryItem>();
	}
}