using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalInspection.Models
{
	public class ChecklistItem
	{
		//[Required(ErrorMessage = "Checklist item name is required")]
		//[DisplayName("Checklist item name *")]
		public string Name { get; set; }

		[Required]
		public IList<Measurement> Measurements { get; set; }

		public enum Condition {NEEDS_SERVICE, SHOULD_WATCH, ALL_GOOD }
		public Guid Id { get; set; }
	}
}