using System.Collections.Generic;
using DigitalInspection.Models.Inspections;

namespace DigitalInspection.ViewModels.Inspections
{
	public class AddMeasurementViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }
		public InspectionItem InspectionItem { get; set; }
		public IList<Measurement> Measurements { get; set; }
	}
}