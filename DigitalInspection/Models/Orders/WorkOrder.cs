using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models.Orders
{
	public class WorkOrder
	{
		[Required(ErrorMessage = "Work Order # is required")]
		[DisplayName("Work Order # *")]
		public string Id { get; set; }

		[Required(ErrorMessage = "Employee # is required")]
		[DisplayName("Employee # *")]
		public string EmployeeId { get; set; }

		public string ServiceAdvisor { get; set; }

		public string ServiceAdvisorName { get; set; }

		public IList<string> BillingSummary { get; set; }

		public string TotalBill { get; set; }

		public IList<string> WorkDescription { get; set; }

		/*
		 * This is a List<string> that will need to be delimited by return carriage whenever we save.
		 */
		public IList<string> Notes { get; set; }

		public DateTime? Date { get; set; }

		public DateTime? ScheduleDate { get; set; }

		public DateTime? CompletionDate { get; set; }

		[Required]
		public WorkOrderStatus Status { get; set; }

		[Required]
		public Customer Customer { get; set; }

		[Required]
		public Vehicle Vehicle { get; set; }

		[Required]
		public IList<RecommendedService> RecommendedServices { get; set; }
	}
}