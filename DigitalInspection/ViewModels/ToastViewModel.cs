namespace DigitalInspection.ViewModels
{
	public enum ToastType { Success, Warn, Error, Info }
	public enum ToastActionType { Close, Unlock, Refresh }

	public class ToastViewModel
	{
		public string Message { get; set; }
		public string Icon { get; set; }
		public ToastType Type { get; set; }
		public ToastActionType Action { get; set; }
	}
}