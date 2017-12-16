using System;
using System.Collections.Generic;

namespace DigitalInspection.ViewModels.TabContainers
{
	public class ScrollableTabContainerViewModel
	{
		public IList<ScrollableTab> Tabs;
	}

	public class ScrollableTab
	{
		public Guid? paneId { get; set; }
		public string title { get; set; }
		public bool active { get; set; }
		public bool disabled { get; set; }
	} 
}