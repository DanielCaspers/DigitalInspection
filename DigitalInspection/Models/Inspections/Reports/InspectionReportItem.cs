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

		public bool IsCustomerConcern { get; set; }

		public InspectionReportItem(InspectionItem ii, string baseUrl)
		{
			InspectionItemId = ii.Id;
			Condition = ii.Condition;
			Note = ii.Note;
			Name = ii.ChecklistItem.Name;
			IsCustomerConcern = ii.IsCustomerConcern;

			CannedResponses = ii.CannedResponses
				.Select(cr => new CannedResponseReportItem(cr.Response, cr.Description, cr.Url, cr.RecommenededServiceDescription))
				.OrderBy(cri => cri.Response);

			Measurements = ii.InspectionMeasurements
				.OrderBy(im => im.Measurement.Label)
				.Select(im => new MeasurementReportItem(im.Value, im.Measurement.Label, im.Measurement.Unit));

			Images = ii.InspectionImages
				.Where(image => image.IsVisibleToCustomer)
				.OrderBy(image => image.CreatedDate)
				.Select(image => new ImageReportItem(baseUrl, ii, image));
		}
	}
}
