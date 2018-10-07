using System;

namespace DigitalInspection.Models.Web
{
	public class UpdateInspectionMeasurementRequest
	{
		public Guid Id { get; set; }

		public int? Value { get; set; }
	}
}