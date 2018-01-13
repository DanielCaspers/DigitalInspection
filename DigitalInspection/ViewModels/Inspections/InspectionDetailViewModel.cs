using DigitalInspection.Models;
using DigitalInspection.ViewModels.TabContainers;
using System;

namespace DigitalInspection.ViewModels
{
	public class InspectionDetailViewModel: BaseInspectionsViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public Checklist Checklist { get; set; }

		// Used for refreshing the view properly in the RedirectToAction() call in the controller after upload
		public Guid? FilteringTagId { get; set; }

		public Inspection Inspection { get; set; }

		public AddMeasurementViewModel AddMeasurementVM { get; set; }

		public AddInspectionNoteViewModel AddInspectionNoteVM { get; set; }

		public UploadInspectionPhotosViewModel UploadInspectionPhotosVM { get; set; }

		public ViewInspectionPhotosViewModel ViewInspectionPhotosVM { get; set; }

		public ScrollableTabContainerViewModel ScrollableTabContainerVM { get; set; }
	}
}