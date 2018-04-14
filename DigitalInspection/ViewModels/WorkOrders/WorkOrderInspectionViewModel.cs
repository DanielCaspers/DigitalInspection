using System;
using System.Collections.Generic;
using DigitalInspection.Models.Inspections;

namespace DigitalInspection.ViewModels.WorkOrders
{
	public class WorkOrderInspectionViewModel: WorkOrderDetailViewModel
	{
		public IList<Checklist> Checklists;

		public Guid InspectionId;
	}
}