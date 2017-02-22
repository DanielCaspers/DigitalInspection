using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalInspection.ViewModels
{
	public class AddChecklistViewModel
	{
		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Picture is requried")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public HttpPostedFileBase Picture { get; set; }
	}
}