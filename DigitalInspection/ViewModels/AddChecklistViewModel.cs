using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalInspection.ViewModels
{
	public class AddChecklistViewModel
	{
		[Required]
		[DisplayName("Checklist name")]
		public string Name { get; set; }

		public HttpPostedFileBase Picture { get; set; }
	}
}