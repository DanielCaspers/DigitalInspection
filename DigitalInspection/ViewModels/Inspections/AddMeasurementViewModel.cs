using System.ComponentModel;
using DigitalInspection.Models;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels
{
	public class AddMeasurementViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }
		public IList<Measurement> Measurements { get; set; }
	}
}