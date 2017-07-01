using DigitalInspection.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{

	public class WorkOrder
	{
		[Required(ErrorMessage = "Work Order # is required")]
		[DisplayName("Work Order # *")]
		public string Id { get; set; }

		// TODO: Add relation to a checklist

		[Required(ErrorMessage = "Employee # is required")]
		[DisplayName("Employee # *")]
		public int EmployeeId { get; set; }

		[Required]
		public WorkOrderStatus Status { get; set; }

		public DateTime? Date { get; set; }

		public DateTime? ScheduleDate { get; set; }

		public DateTime? CompletionDate { get; set; }

		[Required]
		public Customer Customer { get; set; }

		[Required]
		public Vehicle Vehicle { get; set; }

		public IList<string> WorkDescription { get; set; }
	}
}