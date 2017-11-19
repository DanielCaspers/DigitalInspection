using DigitalInspection.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalInspection.ViewModels
{
	public class UploadInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public string WorkOrderId { get; set; }

		[Required(ErrorMessage = "Picture is required")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public HttpPostedFileBase Picture { get; set; }
	}
}