using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspection.Models;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels
{
	public class AddChecklistItemViewModel
	{
		[Required(ErrorMessage = "Checklist item name is required")]
		[DisplayName("Checklist item name *")]
		public string Name { get; set; }


		[DisplayName("Tags *")]
		[Required(ErrorMessage = "One or more tags are required")]
		public List<Tag> Tags { get; set; }
	}
}