using DigitalInspection.Models;
using DigitalInspection.ViewModels.TabContainers;

namespace DigitalInspection.ViewModels
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