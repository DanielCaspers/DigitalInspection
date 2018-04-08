using System.Collections.Generic;

namespace DigitalInspection.Models.Web
{
	public class VehicleHistoryResponse: BaseResponse
	{
		public IList<VehicleHistoryItem> VehicleHistory { get; set; }
	}
}
