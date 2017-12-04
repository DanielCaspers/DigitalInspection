using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class ChecklistItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		// Many to Many navigation property
		public virtual IList<Checklist> Checklists { get; set; } = new List<Checklist>();

		public virtual IList<Inspection> Inspections { get; set; } = new List<Inspection>();

		public virtual IList<InspectionItem> InspectionItems { get; set; } = new List<InspectionItem>();

		//[Required(ErrorMessage = "Checklist item name is required")]
		//[DisplayName("Checklist item name *")]
		public string Name { get; set; }

		[Required]
		public virtual IList<Tag> Tags { get; set; }

		// Virtual lazy loads and makes EF less dumb 
		// http://stackoverflow.com/a/9246932/2831961
		[Required] // TODO: REVISIT REQUIRED
		public virtual IList<CannedResponse> CannedResponses { get; set; } = new List<CannedResponse>();

		[Required]
		public virtual IList<Measurement> Measurements { get; set; } = new List<Measurement>();

		public enum Condition {NEEDS_SERVICE, SHOULD_WATCH, ALL_GOOD }
	}
}