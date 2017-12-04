using DigitalInspection.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspection.Models
{
	public class InspectionItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public virtual Inspection Inspection { get; set; }

		public virtual ChecklistItem ChecklistItem { get; set; }

		public string Note { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public virtual IList<CannedResponse> CannedResponses { get; set; } = new List<CannedResponse>();

		[NotMapped]
		public IList<Guid> SelectedCannedResponseIds { get; set; } = new List<Guid>();
	}
}