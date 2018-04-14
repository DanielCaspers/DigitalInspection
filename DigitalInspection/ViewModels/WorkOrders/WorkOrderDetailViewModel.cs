using DigitalInspection.Models.Orders;
using DigitalInspection.ViewModels.Base;
using DigitalInspection.ViewModels.Inspections;
using DigitalInspection.ViewModels.TabContainers;
using DigitalInspection.ViewModels.VehicleHistory;

namespace DigitalInspection.ViewModels.WorkOrders
{
	public class WorkOrderDetailViewModel: BaseWorkOrdersViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public VehicleHistoryViewModel VehicleHistoryVM { get; set; }

		public AddInspectionWorkOrderNoteViewModel AddInspectionWorkOrderNoteVm { get; set; }

		public bool CanEdit { get; set; } = false;

		public TabContainerViewModel TabViewModel { get; set; }
	}
}