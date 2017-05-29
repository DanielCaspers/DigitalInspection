using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	// TODO: Make stakeholders decide possible statuses
	public enum WorkOrderStatus {
		[Display(Name = "Not Started")]
		NotStarted,

		[Display(Name = "In Progress")]
		InProgress,

		[Display(Name = "Complete")]
		Complete
	}

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
		public Customer Customer { get; set; }

		[Required]
		public Vehicle Vehicle { get; set; }

		// TODO: Required?
		public WorkOrderStatus Status { get; set; }

		// TODO: Add date(s)
		public DateTime Date { get; set; }
	}
}