using System.Net;

namespace DigitalInspection.Models.Web
{
	public enum WorkOrderResponseCode { SUCCESS, NOT_LOCKED, LOCKED_BY_ANOTHER_CLIENT, LOCK_EXPIRED}

	public class WorkOrderResponse
	{
		public WorkOrder WorkOrder { get; set; }

		public string ErrorMessage { get; set; }

		public HttpStatusCode HTTPCode { get; set; }

		public bool IsSuccessStatusCode { get; set; }
	}
}