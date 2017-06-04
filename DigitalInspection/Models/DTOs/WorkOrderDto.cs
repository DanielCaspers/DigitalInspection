
namespace DigitalInspection.Models.DTOs
{
	public class WorkOrderDTO
	{
		public WorkOrderDTO() { }

		public string orderId { get; set; }
		public int orderStatus { get; set; }
		public long orderDate { get; set; }
		public string schedDate { get; set; }
		public string completionDate { get; set; }
		public string clientId { get; set; }
		public string clientName { get; set; }
		public string clientAddr { get; set; }
		public string clientAddr2 { get; set; }
		public string clientCity { get; set; }
		public string clientState { get; set; }
		public string clientZip { get; set; }
		public ClientPhoneDTO[] clientPhone { get; set; }
		public string vehicleId { get; set; }
		public int? vehicleYear { get; set; }
		public string vehicleMake { get; set; }
		public string vehicleModel { get; set; }
		public string vehicleLicense { get; set; }
		public string vehicleColor { get; set; }
		public string vehicleEngine { get; set; }
		public string vehicleTransmission { get; set; }
		public int? vehicleOdometer { get; set; }
		//public string[] workDesc { get; set; }
		// work desc
	}
}