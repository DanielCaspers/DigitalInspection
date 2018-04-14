using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using DigitalInspection.Models.Validators;

namespace DigitalInspection.ViewModels.Checklists
{
	public class AddChecklistViewModel
	{
		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Picture is required")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public HttpPostedFileBase Picture { get; set; }
	}
}