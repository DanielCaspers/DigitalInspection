using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Validators;

namespace DigitalInspection.ViewModels.Inspections
{
	public class UploadInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		// Used for Post-Return-Get
		public Guid ChecklistId { get; set; }
		public Guid? TagId { get; set; }

		// Used for the naming scheme of the saved image for easier recognition outside of DI
		// for the use case of pruning old inspection images which no longer need to be kept on file.
		public string WorkOrderId { get; set; }

		[Required(ErrorMessage = "Picture or video is required")]
		[DisplayName("Attach a picture or video *")]
		[MaxFileSize(100 * 1024 * 1024, ErrorMessage = "Max image or video size is 100 MB")]
		[DataType(DataType.Upload)]
		public HttpPostedFileBase Picture { get; set; }
	}
}