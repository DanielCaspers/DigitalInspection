using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Checklist
	{
		[Required]
		public Guid Id { get; set; }

		// Many to Many navigation property
		public virtual IList<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		//[Required]
		public Image Image { get; set; }

		public virtual IList<Inspection> Inspections { get; set; } = new List<Inspection>();
	}
}