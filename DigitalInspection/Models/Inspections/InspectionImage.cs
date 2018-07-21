namespace DigitalInspection.Models.Inspections
{
	public class InspectionImage : Image
	{
		public virtual InspectionItem InspectionItem { get; set; }

		// Determines if visible in the customer's inspection report
		public bool IsVisibleToCustomer { get; set; } = true;
	}
}
