using System.Collections.Generic;
using DigitalInspection.Models.Inspections;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels.Checklists
{
	public class EditChecklistViewModel: BaseChecklistsViewModel
	{
		public Checklist Checklist { get; set; }
		
		public IList<ChecklistItem> ChecklistItems { get; set; }

		public IList<bool> IsChecklistItemSelected { get; set; }
	}
}