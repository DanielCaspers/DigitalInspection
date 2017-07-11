
using System.Collections.Generic;

namespace DigitalInspection.Models.DTOs
{
	public class WorkOrderDTO
	{
		public WorkOrderDTO() { }

		public string orderID { get; set; }
		public WorkOrderStatusDTO orderStatus { get; set; }
		public long? orderDate { get; set; }
		public long? schedDate { get; set; }
		public long? completionDate { get; set; }
		public int techNum { get; set; }
		public string serviceAdvisor { get; set; }
		public IList<string> billingSummary { get; set; }
		public string totalBill { get; set; }
		public IList<string> workDesc { get; set; }

		public string clientID { get; set; }
		public string clientName { get; set; }
		public string clientAddr { get; set; }
		public string clientAddr2 { get; set; }
		public string clientCity { get; set; }
		public string clientState { get; set; }
		public string clientZip { get; set; }
		public IList<ClientPhoneDTO> clientPhone { get; set; }

		public string vehicleID { get; set; }
		public int? vehicleYear { get; set; }
		public string vehicleMake { get; set; }
		public string vehicleModel { get; set; }
		public string vehicleLicense { get; set; }
		public string vehicleColor { get; set; }
		public string vehicleEngine { get; set; }
		public string vehicleTransmission { get; set; }
		public int? vehicleOdometer { get; set; }
	}
}