namespace DigitalInspection.ViewModels
{
	public class BaseWorkOrdersViewModel : BaseViewModel
	{
		public BaseWorkOrdersViewModel()
		{
			ResourceName = "Work Orders";
			ResourceControllerName = "WorkOrders";
			ResourceMethodName = "Index";
		}
	}
}