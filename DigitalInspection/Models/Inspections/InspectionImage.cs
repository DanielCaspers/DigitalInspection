namespace DigitalInspection.Models.Inspections
{
	public class InspectionImage: Image
	{
		public virtual InspectionItem InspectionItem { get; set; }
	}
}
