using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class ManageChecklistMasterViewModel: BaseViewModel
	{
		public List<Checklist> Checklists { get; set; }
		public AddChecklistViewModel AddChecklistVM { get; set; }
	}
}