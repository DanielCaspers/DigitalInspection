using System.ComponentModel;

namespace DigitalInspection.ViewModels
{
	public class AddInspectionNoteViewModel
	{
		[DisplayName("Note")]
		public string Note { get; set; }
	}
}