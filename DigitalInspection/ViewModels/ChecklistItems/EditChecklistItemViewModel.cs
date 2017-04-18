using DigitalInspection.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.ViewModels
{
	public class EditChecklistItemViewModel: BaseChecklistsViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		[DisplayName("Tags *")]
		[Required(ErrorMessage = "One or more tags are required")]
		public IList<Tag> Tags { get; set; }
	}
}