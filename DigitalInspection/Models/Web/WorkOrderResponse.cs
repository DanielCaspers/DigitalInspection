namespace DigitalInspection.Models.Web
{
	public enum WorkOrderResponseCode { SUCCESS, NOT_LOCKED, LOCKED_BY_ANOTHER_CLIENT, LOCK_EXPIRED}

	public class WorkOrderResponse : BaseResponse
	{
		public WorkOrder WorkOrder { get; set; }
	}
}