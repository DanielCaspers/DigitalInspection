using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Tag
	{
		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required(ErrorMessage = "Tag name is required")]
		[DisplayName("Tag name *")]
		public string Name { get; set; }

		[DisplayName("Is visible to customer? *")]
		public bool IsVisibleToCustomer { get; set; }

		[DisplayName("Is visible to employee? *")]
		public bool IsVisibleToEmployee { get; set; } = true;

		public virtual IList<ChecklistItem> ChecklistItems { get; set; }
	}
}