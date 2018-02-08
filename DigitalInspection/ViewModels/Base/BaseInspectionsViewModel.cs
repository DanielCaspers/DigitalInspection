namespace DigitalInspection.ViewModels
{
	public class BaseInspectionsViewModel : BaseViewModel
	{
		public BaseInspectionsViewModel()
		{
			ResourceName = "Inspections";
			// While not corresponding to inspections, allows quick navigation back to work order table
			ResourceControllerName = "WorkOrders";
			ResourceMethodName = "Index";
		}
	}
}