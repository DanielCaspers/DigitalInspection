namespace DigitalInspection.Models.DTOs
{
	public class WorkOrderStatusDTO
	{
		public int statusCode { get; set; }
		public string statusDesc { get; set; }
		public long statusTimestamp { get; set; }
		public string statusMisc { get; set; }

		public WorkOrderStatusDTO() { }

		public WorkOrderStatusDTO(int code, string desc, long timestamp, string misc)
		{
			statusCode = code;
			statusDesc = desc;
			statusTimestamp = timestamp;
			statusMisc = misc;
		}
	}
}