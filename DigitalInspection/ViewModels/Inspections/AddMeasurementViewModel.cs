using System.ComponentModel;
using DigitalInspection.Models;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels
{
	public class AddMeasurementViewModel
	{
		public IList<Measurement> Measurements { get; set; }
	}
}