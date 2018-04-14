using System.Collections.Generic;
using DigitalInspection.Models.Inspections;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels.Tags
{
	public class ManageTagsViewModel: BaseChecklistsViewModel
	{
		public List<Tag> Tags { get; set; }
		public AddTagViewModel AddTagVM { get; set; }
	}
}