using System;

namespace DigitalInspection.Models
{
	public class InspectionMeasurement
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public virtual InspectionItem InspectionItem { get; set; }

		public virtual Measurement Measurement { get; set; }

		public int? Value { get; set; }
	}
}