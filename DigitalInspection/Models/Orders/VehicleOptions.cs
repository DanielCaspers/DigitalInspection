using System.ComponentModel;

namespace DigitalInspection.Models.Orders
{
	public class VehicleOptions
	{
		[DisplayName("4WD")]
		public bool Has4WD { get; set; }

		[DisplayName("AWD")]
		public bool HasAWD { get; set; }

		[DisplayName("AC")]
		public bool HasAC { get; set; }

		[DisplayName("ABS")]
		public bool HasABS { get; set; }

		public VehicleOptions() { }

		public VehicleOptions(bool? _4WD, bool? _AWD, bool? _AC, bool? _ABS)
		{
			Has4WD = _4WD ?? false;
			HasAWD = _AWD ?? false;
			HasAC = _AC ?? false;
			HasABS = _ABS ?? false;
		}
	}
}
