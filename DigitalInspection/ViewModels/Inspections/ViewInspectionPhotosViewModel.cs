using System;
using System.Collections.Generic;
using DigitalInspection.Models.Inspections;

namespace DigitalInspection.ViewModels.Inspections
{
	public class ViewInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public IList<InspectionImage> Images { get; set; }

		// Used for Post-Return-Get
		public Guid ChecklistId { get; set; }
		public Guid? TagId { get; set; }
		public string WorkOrderId { get; set; }
	}
}