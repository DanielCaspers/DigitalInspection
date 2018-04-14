using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.ViewModels.Tags
{
	public class AddTagViewModel
	{
		[Required(ErrorMessage = "Tag name is required")]
		[DisplayName("Tag name *")]
		public string Name { get; set; }

		[DisplayName("Is visible to customer? *")]
		public bool IsVisibleToCustomer { get; set; }

		[DisplayName("Is visible to employee? *")]
		public bool IsVisibleToEmployee { get; set; } = true;
	}
}