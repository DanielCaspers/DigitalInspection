using System.Collections.Generic;
using DigitalInspection.Models.Orders;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels.WorkOrders
{
	public class WorkOrderMasterViewModel: BaseWorkOrdersViewModel
	{
		public IList<WorkOrder> WorkOrders { get; set; }
	}
}