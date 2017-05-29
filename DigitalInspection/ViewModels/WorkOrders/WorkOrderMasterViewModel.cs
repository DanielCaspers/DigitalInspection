using System.Collections.Generic;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class WorkOrderMasterViewModel: BaseWorkOrdersViewModel
	{
		public IList<WorkOrder> WorkOrders { get; set; }
	}
}