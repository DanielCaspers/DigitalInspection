using System.Collections.Generic;
using DigitalInspection.Models.Inspections;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels.Checklists
{
	public class ManageChecklistMasterViewModel: BaseChecklistsViewModel
	{
		public List<Checklist> Checklists { get; set; }
		public AddChecklistViewModel AddChecklistVM { get; set; }
	}
}