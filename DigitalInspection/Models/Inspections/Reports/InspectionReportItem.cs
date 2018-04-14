using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.Inspections.Reports
{
	public class InspectionReportItem
	{
		public Guid InspectionItemId { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public string Note { get; set; }

		public string Name { get; set; }

		public IEnumerable<CannedResponseReportItem> CannedResponses { get; set; }

		public IEnumerable<MeasurementReportItem> Measurements { get; set; }

		public IEnumerable<ImageReportItem> Images { get; set; }

		public InspectionReportItem(InspectionItem ii, string baseUrl)
		{
			InspectionItemId = ii.Id;
			Condition = ii.Condition;
			Note = ii.Note;
			Name = ii.ChecklistItem.Name;

			CannedResponses = ii.CannedResponses
				.Select(cr => new CannedResponseReportItem(cr.Response, cr.Description, cr.Url));

			Measurements = ii.InspectionMeasurements
				.Select(im => new MeasurementReportItem(im.Value, im.Measurement.Label, im.Measurement.Unit));

			Images = ii.InspectionImages
				.Select(image => new ImageReportItem(baseUrl, ii, image));
		}
	}
}
