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

		// Virtual lazy loads and makes EF less dumb 
		// http://stackoverflow.com/a/9246932/2831961
		[Required]
		public virtual IList<Measurement> Measurements { get; set; }

		public enum Condition {NEEDS_SERVICE, SHOULD_WATCH, ALL_GOOD }
		public Guid Id { get; set; }
	}
}