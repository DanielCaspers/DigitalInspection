namespace DigitalInspection.ViewModels
{
	public class ConfirmDialogViewModel
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public string CancelActionText { get; set; } = "Cancel";
		public string AffirmativeActionText { get; set; }
	}
}
