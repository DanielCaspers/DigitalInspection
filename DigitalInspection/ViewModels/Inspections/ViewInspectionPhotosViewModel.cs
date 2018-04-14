using System.Collections.Generic;
using DigitalInspection.Models.Inspections;

namespace DigitalInspection.ViewModels.Inspections
{
	public class ViewInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public IList<string> ImageSources { get; set; }
	}
}