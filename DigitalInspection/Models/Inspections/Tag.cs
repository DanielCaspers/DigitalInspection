using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Tag
	{
		[Required]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Tag name is required")]
		[DisplayName("Tag name *")]
		public string Name { get; set; }

		public virtual IList<ChecklistItem> ChecklistItems { get; set; }
	}
}