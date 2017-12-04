using System.ComponentModel;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class AddInspectionNoteViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		[DisplayName("Note")]
		public string Note { get; set; }
	}
}