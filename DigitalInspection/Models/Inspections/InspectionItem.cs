using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.Inspections
{
	public class InspectionItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public virtual Inspection Inspection { get; set; }

		public virtual ChecklistItem ChecklistItem { get; set; }

		public virtual IList<InspectionMeasurement> InspectionMeasurements { get; set; } = new List<InspectionMeasurement>();

		public string Note { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public virtual IList<CannedResponse> CannedResponses { get; set; } = new List<CannedResponse>();

		[NotMapped]
		public IList<Guid> SelectedCannedResponseIds { get; set; } = new List<Guid>();

		public virtual IList<InspectionImage> InspectionImages { get; set; } = new List<InspectionImage>();
	}
}