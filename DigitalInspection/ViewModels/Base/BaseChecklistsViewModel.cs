namespace DigitalInspection.ViewModels.Base
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