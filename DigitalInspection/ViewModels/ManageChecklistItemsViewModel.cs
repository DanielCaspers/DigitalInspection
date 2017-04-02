using System.Collections.Generic;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class ManageChecklistItemsViewModel: BaseViewModel
	{
		public List<ChecklistItem> ChecklistItems { get; set; }
		public AddChecklistItemViewModel AddChecklistItemVM { get; set; }
	}
}