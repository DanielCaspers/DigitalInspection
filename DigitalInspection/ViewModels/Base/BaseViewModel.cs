namespace DigitalInspection.ViewModels.Base
{
	public class BaseViewModel
	{
		public string ResourceName { get; set; }

		public string ResourceControllerName { get; set; }

		public string ResourceMethodName { get; set; }

		public ToastViewModel Toast { get; set; }
	}
}