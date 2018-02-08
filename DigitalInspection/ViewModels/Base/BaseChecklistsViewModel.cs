namespace DigitalInspection.ViewModels
{
	public class BaseChecklistsViewModel : BaseViewModel
	{
		public BaseChecklistsViewModel()
		{
			ResourceName = "Checklists";
			ResourceControllerName = "Checklists";
			ResourceMethodName = "Index";
		}
	}
}