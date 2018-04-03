using System;
using DigitalInspection.Utils;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models.Orders
{
	public enum RecommendedServiceSeverity
	{
		// The unset value of the enum should be 0, so that this is also the default constructed value
		// We need this to remain this way so that we can filter these out from the customer's view
		[Display(Name = "Unknown")]
		UNKNOWN = 0,

		[Display(Name = "Immediate")]
		IMMEDIATE = 1,

		[Display(Name = "Moderate")]
		MODERATE = 2,

		[Display(Name = "Should Watch")]
		SHOULD_WATCH = 3,

		[Display(Name = "Maintenance")]
		MAINTENANCE = 4,

		[Display(Name = "Notes")]
		NOTES = 5,

		[Display(Name = "OK")]
		OK = 10,

		[Display(Name = "N/A")]
		NOT_APPLICABLE = 11
	}

	public class RecommendedService
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string OrderId { get; set; }
		public DateTime? LastModifiedDate { get; set; }
		public string CompanyNumber { get; set; }
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
			string severity,
			string companyNumber)
		{
			Id = id;
			Description = desc;
			OrderId = orderId;
			LastModifiedDate = DateTimeUtils.FromUnixTime(lastModifiedDate);
			TechnicianId = technicianId;
			AppLink = appLink;
			EstimateId = estimateId;
			NotificationCount = notificationCount;
			CompanyNumber = companyNumber;

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