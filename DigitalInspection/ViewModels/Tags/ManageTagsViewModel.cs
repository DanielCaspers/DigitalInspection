using System.Collections.Generic;
using DigitalInspection.Models;

namespace DigitalInspection.ViewModels
{
	public class ManageTagsViewModel: BaseChecklistsViewModel
	{
		public List<Tag> Tags { get; set; }
		public AddTagViewModel AddTagVM { get; set; }
	}
}