using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.ViewModels
{
	public class AddTagViewModel
	{
		[Required(ErrorMessage = "Tag name is required")]
		[DisplayName("Tag name *")]
		public string Name { get; set; }
	}
}