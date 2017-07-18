using System;

namespace DigitalInspection.Models.Orders
{
	public class RecommendedService
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string OrderId { get; set; }
		public long? LastModifiedDate { get; set; }
		public string TechnicianId { get; set; }
		public string AppLink { get; set; }
		public string EstimateId { get; set; }
		public int? NotificationCount { get; set; }

		public RecommendedService() { }

		public RecommendedService(
			string id,
			string desc,
			string orderId,
			long? lastModifiedDate,
			string technicianId,
			string appLink,
			string estimateId,
			int? notificationCount)
		{
			Id = id;
			Description = desc;
			OrderId = orderId;
			LastModifiedDate = lastModifiedDate;
			TechnicianId = technicianId;
			AppLink = appLink;
			EstimateId = estimateId;
			NotificationCount = notificationCount;
		}
	}
}