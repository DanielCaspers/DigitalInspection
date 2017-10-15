using DigitalInspection.Models;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels
{
	public class ViewInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public IList<string> ImageSources { get; set; }
	}
}