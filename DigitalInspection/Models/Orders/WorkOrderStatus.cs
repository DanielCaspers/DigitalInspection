
namespace DigitalInspection.Models.Orders
{
	// TODO As DI's responsibility with mutating order statuses increases, properly integrate this with model below
	public enum WorkOrderStatusCode { InspectionCanBeMarkedForCompletion = 15, InspectionCompleted = 22 }

	public class WorkOrderStatus
	{
		public int Code { get; set; }
		public string Description { get; set; }
		public long Timestamp { get; set; }
		public string Misc { get; set; }

		public WorkOrderStatus() { }

		public WorkOrderStatus (int code, string desc, long timestamp, string misc)
		{
			Code = code;
			Description = desc;
			Timestamp = timestamp;
			Misc = misc;
		}
	}
}