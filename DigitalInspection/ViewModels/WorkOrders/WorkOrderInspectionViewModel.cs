using System;
using DigitalInspection.Models;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels
{
	public class WorkOrderInspectionViewModel: WorkOrderDetailViewModel
	{
		public IList<Checklist> Checklists;

		public Guid InspectionId;
	}
}