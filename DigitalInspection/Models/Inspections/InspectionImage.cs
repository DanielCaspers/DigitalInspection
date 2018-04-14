namespace DigitalInspection.Models
{
	public class InspectionImage: Image
	{
		public virtual InspectionItem InspectionItem { get; set; }
	}
}
