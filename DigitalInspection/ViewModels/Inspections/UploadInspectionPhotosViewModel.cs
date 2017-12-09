using DigitalInspection.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System;

namespace DigitalInspection.ViewModels
{
	public class UploadInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		// Used for refreshing the view properly in the RedirectToAction() call in the controller after upload
		public Guid ChecklistId { get; set; }

		// Used for the naming scheme of the saved image for easier recognition outside of DI
		// for the use case of pruning old inspection images which no longer need to be kept on file.
		public string WorkOrderId { get; set; }

		[Required(ErrorMessage = "Picture is required")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public HttpPostedFileBase Picture { get; set; }
	}
}