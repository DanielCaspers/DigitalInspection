using System;
using System.Collections.Generic;

namespace DigitalInspection.Models
{
	public class Inspection
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string WorkOrderId { get; set; }

		public virtual IList<Checklist> Checklists { get; set; } = new List<Checklist>();

		public virtual IList<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

		public virtual IList<InspectionItem> InspectionItems { get; set; } = new List<InspectionItem>();
	}
}