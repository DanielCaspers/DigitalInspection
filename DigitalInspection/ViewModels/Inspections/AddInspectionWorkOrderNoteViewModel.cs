using System.ComponentModel;

namespace DigitalInspection.ViewModels
{
	public class AddInspectionWorkOrderNoteViewModel
	{
		public string WorkOrderId { get; set; }

		[DisplayName("Note")]
		public string Note { get; set; }
	}
}