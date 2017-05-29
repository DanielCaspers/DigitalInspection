using System.Collections.Generic;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class ManageChecklistMasterViewModel: BaseChecklistsViewModel
	{
		public List<Checklist> Checklists { get; set; }
		public AddChecklistViewModel AddChecklistVM { get; set; }
	}
}