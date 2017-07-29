using System;
using DigitalInspection.Utils;

namespace DigitalInspection.Models.Orders
{
	public enum RecommendedServiceSeverity
	{
		IMMEDIATE = 1,
		MODERATE = 2,
		SHOULD_WATCH = 3,
		MAINTENANCE = 4,
		NOTES = 5,
		UNKNOWN = 6
	}

	public class RecommendedService
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string OrderId { get; set; }
		public DateTime? LastModifiedDate { get; set; }
		public string TechnicianId { get; set; }
		public string AppLink { get; set; }
		public string EstimateId { get; set; }
		public int? NotificationCount { get; set; }
		public RecommendedServiceSeverity Severity { get; set; }
		public bool IsCustomerConcern { get; set; }

		public RecommendedService() { }

		public RecommendedService(
			string id,
			string desc,
			string orderId,
			long? lastModifiedDate,
			string technicianId,
			string appLink,
			string estimateId,
			int? notificationCount,
			string severity)
		{
			Id = id;
			Description = desc;
			OrderId = orderId;
			LastModifiedDate = DateTimeUtils.FromUnixTime(lastModifiedDate);
			TechnicianId = technicianId;
			AppLink = appLink;
			EstimateId = estimateId;
			NotificationCount = notificationCount;

			if (severity == string.Empty)
			{
				IsCustomerConcern = false;
				Severity = RecommendedServiceSeverity.UNKNOWN;
			}
			else
			{
				IsCustomerConcern = severity.StartsWith("0");
				Severity = (RecommendedServiceSeverity) Convert.ToInt32(severity);
			}
		}
	}
}