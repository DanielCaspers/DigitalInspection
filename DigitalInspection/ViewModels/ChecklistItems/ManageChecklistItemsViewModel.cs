using System.Collections.Generic;
using DigitalInspection.Models.Inspections;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels.ChecklistItems
{
	public class ManageChecklistItemsViewModel: BaseChecklistsViewModel
	{
		public List<ChecklistItem> ChecklistItems { get; set; }
		public AddChecklistItemViewModel AddChecklistItemVM { get; set; }
	}
}