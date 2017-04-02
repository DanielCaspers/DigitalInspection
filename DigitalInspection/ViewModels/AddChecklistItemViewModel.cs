using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalInspection.ViewModels
{
	public class AddChecklistItemViewModel
	{
		[Required(ErrorMessage = "Checklist item name is required")]
		[DisplayName("Checklist item name *")]
		public string Name { get; set; }
	}
}