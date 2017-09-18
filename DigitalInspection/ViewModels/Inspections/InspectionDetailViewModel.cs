using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class InspectionDetailViewModel: BaseInspectionsViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public Checklist Checklist { get; set; }

		public string InspectionId { get; set; }

		public AddMeasurementViewModel AddMeasurementVM { get; set; }

		public AddInspectionNoteViewModel AddInspectionNoteVM { get; set; }

		public UploadInspectionPhotosViewModel UploadInspectionPhotosVM { get; set; }
	}
}