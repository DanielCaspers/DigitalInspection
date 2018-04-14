using System;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Orders;
using DigitalInspection.ViewModels.Base;
using DigitalInspection.ViewModels.TabContainers;
using DigitalInspection.ViewModels.VehicleHistory;

namespace DigitalInspection.ViewModels.Inspections
{
	public class InspectionDetailViewModel: BaseInspectionsViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public Checklist Checklist { get; set; }

		// Used for refreshing the view properly in the RedirectToAction() call in the controller after upload
		public Guid? FilteringTagId { get; set; }

		public Inspection Inspection { get; set; }

		public AddMeasurementViewModel AddMeasurementVM { get; set; }

		public AddInspectionItemNoteViewModel AddInspectionItemNoteVm { get; set; }

		public AddInspectionWorkOrderNoteViewModel AddInspectionWorkOrderNoteVm { get; set; }

		public UploadInspectionPhotosViewModel UploadInspectionPhotosVM { get; set; }

		public ViewInspectionPhotosViewModel ViewInspectionPhotosVM { get; set; }

		public VehicleHistoryViewModel VehicleHistoryVM { get; set; }

		public ScrollableTabContainerViewModel ScrollableTabContainerVM { get; set; }
	}
}