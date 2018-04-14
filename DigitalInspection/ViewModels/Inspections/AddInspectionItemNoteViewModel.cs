using System.ComponentModel;
using DigitalInspection.Models.Inspections;

namespace DigitalInspection.ViewModels.Inspections
{
	public class AddInspectionItemNoteViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		[DisplayName("Note")]
		public string Note { get; set; }
	}
}