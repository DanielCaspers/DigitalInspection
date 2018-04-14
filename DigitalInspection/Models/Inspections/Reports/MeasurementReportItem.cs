namespace DigitalInspection.Models.Inspections.Reports
{
	public class MeasurementReportItem
	{
		public int? Value { get; set; }

		public string Label { get; set; }

		public string Unit { get; set; }

		public MeasurementReportItem(int? value, string label, string unit)
		{
			Value = value;
			Label = label;
			Unit = unit;
		}
	}
}
