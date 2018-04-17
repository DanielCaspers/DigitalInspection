using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.DTOs
{
	public class VehicleOptionsDTO
	{
		public bool? has4WD { get; set; }
		public bool? hasAWD { get; set; }
		public bool? hasAC { get; set; }
		public bool? hasABS { get; set; }

		public VehicleOptionsDTO() { }

		public VehicleOptionsDTO(VehicleOptions options) : this(options.Has4WD, options.HasAWD, options.HasAC, options.HasABS)
		{
		}

		public VehicleOptionsDTO(bool? _4WD, bool? _AWD, bool? _AC, bool? _ABS)
		{
			has4WD = _4WD;
			hasAWD = _AWD;
			hasAC = _AC;
			hasABS = _ABS;
		}
	}
}
