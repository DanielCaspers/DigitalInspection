using System.Collections.Generic;
using DigitalInspection.Models.Web;

namespace DigitalInspection.ViewModels
{
	public class VehicleHistoryViewModel
	{
		public IList<VehicleHistoryItem> VehicleHistory { get; set; } = new List<VehicleHistoryItem>();
	}
}