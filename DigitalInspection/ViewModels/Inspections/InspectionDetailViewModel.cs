using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class InspectionDetailViewModel: BaseInspectionsViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public Checklist Checklist { get; set; }

		// TODO: Remove. Not sure why added separately
		public string InspectionId { get; set; }

		public Inspection Inspection { get; set; }

		public AddMeasurementViewModel AddMeasurementVM { get; set; }

		public AddInspectionNoteViewModel AddInspectionNoteVM { get; set; }

		public UploadInspectionPhotosViewModel UploadInspectionPhotosVM { get; set; }

		public ViewInspectionPhotosViewModel ViewInspectionPhotosVM { get; set; }
	}
}